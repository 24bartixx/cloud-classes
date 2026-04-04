using Rewards.Infrastructure.Bus;
using Rewards.Service.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Rewards.Service");
        services.AddPersistence(ctx.Configuration);
        services.AddHostedService<Rewards.Service.Workers.RewardsWorker>();
    })
    .Build();

await host.RunAsync();
