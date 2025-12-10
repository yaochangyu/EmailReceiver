using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Entities;

namespace EmailReceiver.WebApi.Repositories;

public interface IEmailMessageRepository
{
    Task<Result<EmailMessage>> AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByUidlAsync(string uidl, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<EmailMessage>>> GetAllAsync(CancellationToken cancellationToken = default);
}
