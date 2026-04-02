using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Logging;

namespace Shared.Messaging;

public static class ServiceCollectionExtensions
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
        services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();

        return services;
    }
}
