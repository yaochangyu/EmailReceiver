using EmailReceiver.WebApi.Infrastructure;

namespace EmailReceiver.IntegrationTest;

class TestAssistant
{
    public static void SetEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    }

    public static void SetDbConnectionEnvironmentVariable(string connectionString)
    {
        Environment.SetEnvironmentVariable(nameof(SYS_DATABASE_CONNECTION_STRING), connectionString);
    }

    public static void SetRedisConnectionEnvironmentVariable(string url)
    {
        // Environment.SetEnvironmentVariable(nameof(SYS_REDIS_URL), url);
    }

    public static void SetExternalConnectionEnvironmentVariable(string url)
    {
        // Environment.SetEnvironmentVariable(nameof(EXTERNAL_API), url);
    }

    public static DateTime ToUtc(string time)
    {
        var tempTime = DateTimeOffset.Parse(time);
        var utcTime = new DateTimeOffset(tempTime.DateTime, TimeSpan.Zero).UtcDateTime;
        return utcTime;
    }
}