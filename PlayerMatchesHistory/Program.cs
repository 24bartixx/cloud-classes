using PlayerMatchesHistory.Infrastructure.Bus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "PlayerMatchesHistory.Service");
        services.AddScoped<PlayerMatchesHistory.Service.Services.IPlayerMatchesHistoryService, PlayerMatchesHistory.Service.Services.PlayerMatchesHistoryService>();
        services.AddHostedService<PlayerMatchesHistory.Service.Workers.PlayerMatchesHistoryWorker>();
    })
    .Build();

await host.RunAsync();
