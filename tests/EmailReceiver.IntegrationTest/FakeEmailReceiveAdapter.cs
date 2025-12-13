using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Adpaters;
using EmailReceiver.WebApi.EmailReceiver.Models.Responses;
using EmailReceiver.WebApi.Infrastructure.ErrorHandling;

namespace EmailReceiver.IntegrationTest;

public class FakeEmailReceiveAdapter : IEmailReceiveAdapter
{
    private readonly List<EmailMessageResponse> _emails = new();
    private Failure? _failure;

    public void SetEmails(IEnumerable<EmailMessageResponse> emails)
    {
        _emails.Clear();
        _emails.AddRange(emails);
    }

    public void SetFailure(Failure failure)
    {
        _failure = failure;
    }

    public void Clear()
    {
        _emails.Clear();
        _failure = null;
    }

    public Task<Result<IReadOnlyList<EmailMessageResponse>, Failure>> FetchEmailsAsync(CancellationToken cancellationToken = default)
    {
        if (_failure != null)
        {
            return Task.FromResult(Result.Failure<IReadOnlyList<EmailMessageResponse>, Failure>(_failure));
        }

        return Task.FromResult(Result.Success<IReadOnlyList<EmailMessageResponse>, Failure>(_emails.AsReadOnly()));
    }
}
