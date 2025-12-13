using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Models.Responses;
using EmailReceiver.WebApi.EmailReceiver.Options;
using MailKit.Net.Pop3;
using Microsoft.Extensions.Options;

namespace EmailReceiver.WebApi.EmailReceiver.Adpaters;

public class Pop3EmailReceiveAdapter : IEmailReceiveAdapter
{
    private readonly Pop3Options _options;
    private readonly ILogger<Pop3EmailReceiveAdapter> _logger;

    public Pop3EmailReceiveAdapter(
        IOptions<Pop3Options> options,
        ILogger<Pop3EmailReceiveAdapter> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<EmailMessageResponse>>> FetchEmailsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new Pop3Client();

            await client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, cancellationToken);
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);

            var emails = new List<EmailMessageResponse>();
            var count = await client.GetMessageCountAsync(cancellationToken);

            _logger.LogInformation("POP3 伺服器上有 {Count} 封郵件", count);

            for (var i = 0; i < count; i++)
            {
                var message = await client.GetMessageAsync(i, cancellationToken);
                var uidl = await client.GetMessageUidAsync(i, cancellationToken);

                var emailDto = new EmailMessageResponse(
                    Id: Guid.NewGuid(),
                    Uidl: uidl,
                    Subject: message.Subject ?? string.Empty,
                    Body: message.TextBody ?? message.HtmlBody ?? string.Empty,
                    From: message.From.ToString(),
                    To: message.To.ToString(),
                    ReceivedAt: message.Date.UtcDateTime,
                    CreatedAt: DateTime.UtcNow
                );

                emails.Add(emailDto);
            }

            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("成功從 POP3 伺服器取得 {Count} 封郵件", emails.Count);

            return Result.Success<IReadOnlyList<EmailMessageResponse>>(emails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "從 POP3 伺服器取得郵件時發生錯誤");
            return Result.Failure<IReadOnlyList<EmailMessageResponse>>($"從 POP3 伺服器取得郵件時發生錯誤: {ex.Message}");
        }
    }
}
