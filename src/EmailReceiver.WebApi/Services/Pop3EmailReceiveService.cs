using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Models;
using EmailReceiver.WebApi.Options;
using MailKit.Net.Pop3;
using Microsoft.Extensions.Options;

namespace EmailReceiver.WebApi.Services;

public class Pop3EmailReceiveService : IEmailReceiveService
{
    private readonly Pop3Options _options;
    private readonly ILogger<Pop3EmailReceiveService> _logger;

    public Pop3EmailReceiveService(
        IOptions<Pop3Options> options,
        ILogger<Pop3EmailReceiveService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<EmailDto>>> FetchEmailsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new Pop3Client();

            await client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, cancellationToken);
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);

            var emails = new List<EmailDto>();
            var count = await client.GetMessageCountAsync(cancellationToken);

            _logger.LogInformation("POP3 伺服器上有 {Count} 封郵件", count);

            for (var i = 0; i < count; i++)
            {
                var message = await client.GetMessageAsync(i, cancellationToken);
                var uidl = await client.GetMessageUidAsync(i, cancellationToken);

                var emailDto = new EmailDto(
                    Uidl: uidl,
                    Subject: message.Subject ?? string.Empty,
                    Body: message.TextBody ?? message.HtmlBody ?? string.Empty,
                    From: message.From.ToString(),
                    To: message.To.ToString(),
                    ReceivedAt: message.Date.UtcDateTime
                );

                emails.Add(emailDto);
            }

            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("成功從 POP3 伺服器取得 {Count} 封郵件", emails.Count);

            return Result.Success<IReadOnlyList<EmailDto>>(emails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "從 POP3 伺服器取得郵件時發生錯誤");
            return Result.Failure<IReadOnlyList<EmailDto>>($"從 POP3 伺服器取得郵件時發生錯誤: {ex.Message}");
        }
    }
}
