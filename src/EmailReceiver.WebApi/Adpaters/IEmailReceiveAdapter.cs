using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Models.Responses;

namespace EmailReceiver.WebApi.Adpaters;

public interface IEmailReceiveAdapter
{
    Task<Result<IReadOnlyList<EmailMessageResponse>>> FetchEmailsAsync(CancellationToken cancellationToken = default);
}
