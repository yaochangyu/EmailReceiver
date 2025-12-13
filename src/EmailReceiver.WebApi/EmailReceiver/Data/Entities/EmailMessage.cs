namespace EmailReceiver.WebApi.EmailReceiver.Data.Entities;

public sealed class EmailMessage
{
    public Guid Id { get; init; }
    public string Uidl { get; init; }
    public string Subject { get; init; }
    public string Body { get; init; }
    public string From { get; init; }
    public string To { get; init; }
    public DateTime ReceivedAt { get; init; }
    public DateTime CreatedAt { get; init; }

    private EmailMessage()
    {
        Uidl = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
        From = string.Empty;
        To = string.Empty;
    }

    public static EmailMessage Create(
        string uidl,
        string subject,
        string body,
        string from,
        string to,
        DateTime receivedAt)
    {
        return new EmailMessage
        {
            Id = Guid.NewGuid(),
            Uidl = uidl,
            Subject = subject,
            Body = body,
            From = from,
            To = to,
            ReceivedAt = receivedAt,
            CreatedAt = DateTime.UtcNow
        };
    }
}
