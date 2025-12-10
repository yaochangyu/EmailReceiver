using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Entities;
using EmailReceiver.WebApi.Repositories;
using EmailReceiver.WebApi.Services;

namespace EmailReceiver.WebApi.Handlers;

public class ReceiveEmailsHandler
{
    private readonly IEmailReceiveService _emailReceiveService;
    private readonly IEmailMessageRepository _repository;
    private readonly ILogger<ReceiveEmailsHandler> _logger;

    public ReceiveEmailsHandler(
        IEmailReceiveService emailReceiveService,
        IEmailMessageRepository repository,
        ILogger<ReceiveEmailsHandler> logger)
    {
        _emailReceiveService = emailReceiveService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<int>> HandleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("開始接收郵件");

        var fetchResult = await _emailReceiveService.FetchEmailsAsync(cancellationToken);
        if (fetchResult.IsFailure)
        {
            return Result.Failure<int>(fetchResult.Error);
        }

        var emails = fetchResult.Value;
        var savedCount = 0;

        foreach (var emailDto in emails)
        {
            var existsResult = await _repository.ExistsByUidlAsync(emailDto.Uidl, cancellationToken);
            if (existsResult.IsFailure)
            {
                _logger.LogWarning("檢查 UIDL {Uidl} 是否存在時發生錯誤: {Error}", emailDto.Uidl, existsResult.Error);
                continue;
            }

            if (existsResult.Value)
            {
                _logger.LogInformation("郵件 UIDL {Uidl} 已存在，跳過", emailDto.Uidl);
                continue;
            }

            var emailMessage = EmailMessage.Create(
                emailDto.Uidl,
                emailDto.Subject,
                emailDto.Body,
                emailDto.From,
                emailDto.To,
                emailDto.ReceivedAt
            );

            var addResult = await _repository.AddAsync(emailMessage, cancellationToken);
            if (addResult.IsSuccess)
            {
                savedCount++;
                _logger.LogInformation("成功儲存郵件 UIDL: {Uidl}, 主旨: {Subject}", emailDto.Uidl, emailDto.Subject);
            }
            else
            {
                _logger.LogWarning("儲存郵件 UIDL {Uidl} 時發生錯誤: {Error}", emailDto.Uidl, addResult.Error);
            }
        }

        _logger.LogInformation("郵件接收完成，總共儲存 {SavedCount} 封郵件", savedCount);

        return Result.Success(savedCount);
    }
}
