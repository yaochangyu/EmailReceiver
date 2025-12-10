using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Models.Responses;

namespace EmailReceiver.WebApi.Services;

public interface IEmailReceiveService
{
    Task<Result<IReadOnlyList<EmailMessageResponse>>> FetchEmailsAsync(CancellationToken cancellationToken = default);
}
