using ClanGames.Infrastructure.Configuration;
using ClanGames.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClanGames.Infrastructure.Bus;

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

        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
}
