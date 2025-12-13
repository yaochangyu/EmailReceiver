namespace EmailReceiver.WebApi.EmailReceiver.Models.Responses;

public sealed record EmailMessageResponse(
    Guid Id,
    string Uidl,
    string Subject,
    string Body,
    string From,
    string To,
    DateTime ReceivedAt,
    DateTime CreatedAt
);
