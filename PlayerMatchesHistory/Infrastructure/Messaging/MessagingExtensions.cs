using PlayerMatchesHistory.Infrastructure.Configuration;
using PlayerMatchesHistory.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PlayerMatchesHistory.Infrastructure.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        string serviceName)
    {
        var settings = new RabbitMqSettings();
        services.AddSingleton(settings);

        var loggerFactory = AppLogger.CreateLoggerFactory(serviceName);
        services.AddSingleton<ILoggerFactory>(loggerFactory);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();

        return services;
    }
}
