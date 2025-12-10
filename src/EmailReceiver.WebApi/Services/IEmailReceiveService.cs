using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Models;

namespace EmailReceiver.WebApi.Services;

public interface IEmailReceiveService
{
    Task<Result<IReadOnlyList<EmailDto>>> FetchEmailsAsync(CancellationToken cancellationToken = default);
}
