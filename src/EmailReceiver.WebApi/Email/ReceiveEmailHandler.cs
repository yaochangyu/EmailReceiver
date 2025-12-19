using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Adpaters;
using EmailReceiver.WebApi.EmailReceiver.Models;
using EmailReceiver.WebApi.EmailReceiver.Repositories;
using EmailReceiver.WebApi.Infrastructure.ErrorHandling;

namespace EmailReceiver.WebApi.EmailReceiver;

public class ReceiveEmailHandler
{
    private readonly IEmailReceiveAdapter _emailReceiveAdapter;
    private readonly IReceiveEmailRepository _repository;

    public ReceiveEmailHandler(
        IEmailReceiveAdapter emailReceiveAdapter,
        IReceiveEmailRepository repository)
    {
        _emailReceiveAdapter = emailReceiveAdapter;
        _repository = repository;
    }

    public async Task<Result<int, Failure>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var fetchResult = await _emailReceiveAdapter.FetchEmailsAsync(cancellationToken);
        if (fetchResult.IsFailure)
        {
            return Result.Failure<int, Failure>(fetchResult.Error);
        }

        var emails = fetchResult.Value;
        if (emails.Count == 0)
        {
            return Result.Success<int, Failure>(0);
        }

        /* 目前沒有 Uidl 欄位，暫不檢查是否有重複收信
        var uidlsResult = await _repository.GetAllUidlsAsync(cancellationToken);
        if (uidlsResult.IsFailure)
        {
            return Result.Failure<int, Failure>(uidlsResult.Error);
        }

        // 過濾出新郵件（尚未儲存的郵件）
        var existingUidls = new HashSet<string>(uidlsResult.Value);
        var newEmails = emails.Where(e => !existingUidls.Contains(e.Uidl)).ToList();

        if (newEmails.Count == 0)
        {
            return Result.Success<int, Failure>(0);
        }
       */

        // 寫入到 letters 和 mailReplay 資料庫
        var savedCount = 0;
        var failures = new List<Failure>();

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
                failures.Add(saveResult.Error);
                continue;
            }

            savedCount++;
        }

        // 如果所有郵件都儲存失敗，返回錯誤
        if (savedCount == 0 && failures.Count > 0)
        {
            return Result.Failure<int, Failure>(
                Failure.DbError("所有郵件儲存失敗", failures.First().Exception));
        }

        return Result.Success<int, Failure>(savedCount);
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