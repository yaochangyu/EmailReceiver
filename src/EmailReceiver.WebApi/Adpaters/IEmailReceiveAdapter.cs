using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Models.Responses;

namespace EmailReceiver.WebApi.Adpaters;

public interface IEmailReceiveAdapter
{
    Task<Result<IReadOnlyList<EmailMessageResponse>>> FetchEmailsAsync(CancellationToken cancellationToken = default);
}
