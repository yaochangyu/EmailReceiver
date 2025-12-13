using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Data.Entities;

namespace EmailReceiver.WebApi.EmailReceiver.Repositories;

public interface IReceiveEmailRepository
{
    Task<Result<EmailMessage>> AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task<Result> AddRangeAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<EmailMessage>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<string>>> GetAllUidlsAsync(CancellationToken cancellationToken = default);
}
