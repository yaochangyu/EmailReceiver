namespace EmailReceiver.WebApi.Options;

public sealed class Pop3Options
{
    public const string SectionName = "Pop3";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public bool UseSsl { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
