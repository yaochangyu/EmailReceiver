using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Entities;

namespace EmailReceiver.WebApi.Repositories;

public interface IReceiveEmailRepository
{
    Task<Result<EmailMessage>> AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task<Result> AddRangeAsync(IEnumerable<EmailMessage> emailMessages, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<EmailMessage>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<string>>> GetAllUidlsAsync(CancellationToken cancellationToken = default);
}
