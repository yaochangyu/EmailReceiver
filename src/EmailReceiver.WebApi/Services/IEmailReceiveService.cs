using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Models;

namespace EmailReceiver.WebApi.Services;

public interface IEmailReceiveService
{
    Task<Result<IReadOnlyList<EmailContent>>> FetchEmailsAsync(CancellationToken cancellationToken = default);
}
