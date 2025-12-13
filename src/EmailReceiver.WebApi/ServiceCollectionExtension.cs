using EmailReceiver.WebApi.EmailReceiver.Data;
using EmailReceiver.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.WebApi;

public static class ServiceCollectionExtension
{
    public static void AddDatabase(this IServiceCollection services)
    {
        services.AddDbContextFactory<EmailReceiverDbContext>((provider,
            builder) =>
        {
            var environment = provider.GetService<SYS_DATABASE_CONNECTION_STRING>();
            var connectionString = environment.Value;
            builder.UseSqlServer(connectionString)
                .UseLoggerFactory(provider.GetService<ILoggerFactory>())
                .EnableSensitiveDataLogging()
                ;
        });
    }

    public static IServiceCollection AddSysEnvironments(this IServiceCollection services)
    {
        services.AddSingleton<SYS_DATABASE_CONNECTION_STRING>();
        // services.AddSingleton<SYS_REDIS_URL>();
        return services;
    }
}