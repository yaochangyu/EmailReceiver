using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Adpaters;
using EmailReceiver.WebApi.EmailReceiver.Data.Entities;
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

        var uidlsResult = await _repository.GetAllUidlsAsync(cancellationToken);
        if (uidlsResult.IsFailure)
        {
            return Result.Failure<int>(uidlsResult.Error);
        }

        var existingUidls = new HashSet<string>(uidlsResult.Value);

        var newEmails = emails
            .Where(e => !existingUidls.Contains(e.Uidl))
            .Select(e => EmailMessage.Create(
                e.Uidl,
                e.Subject,
                e.Body,
                e.From,
                e.To,
                e.ReceivedAt
            ))
            .ToList();

        if (newEmails.Count == 0)
        {
            _logger.LogInformation("沒有新的郵件");
            return Result.Success(0);
        }

        var addResult = await _repository.AddRangeAsync(newEmails, cancellationToken);
        if (addResult.IsFailure)
        {
            return Result.Failure<int>(addResult.Error);
        }

        _logger.LogInformation("郵件接收完成，總共儲存 {SavedCount} 封郵件", newEmails.Count);

        return Result.Success(newEmails.Count);
    }
}
