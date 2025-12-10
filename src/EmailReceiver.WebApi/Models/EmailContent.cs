namespace EmailReceiver.WebApi.Models;

public sealed record EmailContent(
    string Uidl,
    string Subject,
    string Body,
    string From,
    string To,
    DateTime ReceivedAt
);
