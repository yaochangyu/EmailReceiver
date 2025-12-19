using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Models.Responses;
using EmailReceiver.WebApi.Infrastructure.ErrorHandling;

namespace EmailReceiver.WebApi.EmailReceiver.Adpaters;

public interface IEmailReceiveAdapter
{
    Task<Result<IReadOnlyList<EmailMessageResponse>, Failure>> FetchEmailsAsync(CancellationToken cancellationToken = default);
}
