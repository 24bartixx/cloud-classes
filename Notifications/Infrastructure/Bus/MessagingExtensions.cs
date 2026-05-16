using Notifications.Infrastructure.Configuration;
using Notifications.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Notifications.Infrastructure.Bus;

public static class MessagingExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        var connectionUrl = configuration["RabbitMq:ConnectionUrl"];
        if (string.IsNullOrWhiteSpace(connectionUrl))
        {
            throw new InvalidOperationException("RabbitMq:ConnectionUrl configuration is missing.");
        }

        var settings = new RabbitMqSettings { ConnectionUrl = connectionUrl };
        services.AddSingleton(settings);

        var loggerFactory = AppLogger.CreateLoggerFactory(serviceName);
        services.AddSingleton<ILoggerFactory>(loggerFactory);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();

        return services;
    }
}
