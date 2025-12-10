namespace EmailReceiver.WebApi.Models.Responses;

public sealed record ReceiveEmailsResponse(
    int SavedCount,
    string Message
);
