using PlayerMatchesHistory.Infrastructure.Bus;
using MediatR;
using PlayerMatchesHistory.Application.Commands.SaveMatch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "PlayerMatchesHistory.Service");
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<SaveMatchCommand>());
        services.AddHostedService<PlayerMatchesHistory.Service.Workers.PlayerMatchesHistoryWorker>();
    })
    .Build();

await host.RunAsync();
