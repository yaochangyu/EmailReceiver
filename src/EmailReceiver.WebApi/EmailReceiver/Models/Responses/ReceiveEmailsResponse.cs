namespace EmailReceiver.WebApi.EmailReceiver.Models.Responses;

public sealed record ReceiveEmailsResponse(
    int SavedCount,
    string Message
);
