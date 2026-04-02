using PlayerMatchesHistory.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "PlayerMatchesHistory.Service");
        services.AddHostedService<PlayerMatchesHistory.Service.Workers.PlayerMatchesHistoryWorker>();
    })
    .Build();

await host.RunAsync();
