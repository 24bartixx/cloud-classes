using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace PlayerMatchesHistory.Infrastructure.Logging;

public static class AppLogger
{
    public static ILoggerFactory CreateLoggerFactory(
        string serviceName,
        LogEventLevel minimumLevel = LogEventLevel.Information)
    {
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .Enrich.WithProperty("Service", serviceName)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        return LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(serilogLogger, dispose: true);
        });
    }

    public static ILogger<T> CreateLogger<T>(string serviceName) =>
        CreateLoggerFactory(serviceName).CreateLogger<T>();
}
