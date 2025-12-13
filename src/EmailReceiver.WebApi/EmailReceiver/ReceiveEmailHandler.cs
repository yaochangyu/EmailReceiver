using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Adpaters;
using EmailReceiver.WebApi.EmailReceiver.Models;
using EmailReceiver.WebApi.EmailReceiver.Repositories;

namespace EmailReceiver.WebApi.EmailReceiver;

public class ReceiveEmailHandler
{
    private readonly IEmailReceiveAdapter _emailReceiveAdapter;
    private readonly IReceiveEmailRepository _repository;
    private readonly ILogger<ReceiveEmailHandler> _logger;

    public ReceiveEmailHandler(
        IEmailReceiveAdapter emailReceiveAdapter,
        IReceiveEmailRepository repository,
        ILogger<ReceiveEmailHandler> logger)
    {
        _emailReceiveAdapter = emailReceiveAdapter;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<int>> HandleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("開始接收郵件");

        var fetchResult = await _emailReceiveAdapter.FetchEmailsAsync(cancellationToken);
        if (fetchResult.IsFailure)
        {
            return Result.Failure<int>(fetchResult.Error);
        }

        var emails = fetchResult.Value;
        if (emails.Count == 0)
        {
            _logger.LogInformation("沒有新的郵件");
            return Result.Success(0);
        }

        /* 目前沒有 Uidl 欄位，暫不檢查是否有重複收信
        var uidlsResult = await _repository.GetAllUidlsAsync(cancellationToken);
        if (uidlsResult.IsFailure)
        {
            return Result.Failure<int>(uidlsResult.Error);
        }

        // 過濾出新郵件（尚未儲存的郵件）
        var existingUidls = new HashSet<string>(uidlsResult.Value);
        var emails = emails.Where(e => !existingUidls.Contains(e.Uidl)).ToList();

        if (emails.Count == 0)
        {
            _logger.LogInformation("所有郵件皆已存在，無需儲存");
            return Result.Success(0);
        }
        _logger.LogInformation("發現 {Count} 封新郵件，準備寫入資料庫", emails.Count);
       */

        // 寫入到 letters 和 mailReplay 資料庫
        var savedCount = 0;
        foreach (var email in emails)
        {
            var insertEmailRequest = InsertEmailRequest.Create(
                senderName: ExtractNameFromEmail(email.From),
                senderEmail: email.From,
                subject: email.Subject,
                body: email.Body,
                mailDate: email.ReceivedAt
            );

            var saveResult = await _repository.AddAsync(insertEmailRequest, cancellationToken);
            if (saveResult.IsFailure)
            {
                _logger.LogError("儲存郵件失敗: {Error}", saveResult.Error);
                continue;
            }

            savedCount++;
            _logger.LogInformation("成功儲存郵件 (UIDL: {Uidl}, LNo: {LNo})", email.Uidl, saveResult.Value);
        }

        _logger.LogInformation("完成郵件接收，成功儲存 {SavedCount}/{TotalCount} 封郵件", savedCount, emails.Count);
        return Result.Success(savedCount);
    }

    /// <summary>
    /// 從 Email 地址中提取姓名部分
    /// 例如: "John Doe &lt;john@example.com&gt;" -> "John Doe"
    /// 或: "john@example.com" -> "john@example.com"
    /// </summary>
    private static string ExtractNameFromEmail(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return string.Empty;

        var match = System.Text.RegularExpressions.Regex.Match(emailAddress, @"^(.+?)\s*<.+>$");
        return match.Success ? match.Groups[1].Value.Trim() : emailAddress;
    }
}