using Rewards.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Rewards.Service");
        services.AddHostedService<Rewards.Service.Workers.RewardsWorker>();
    })
    .Build();

await host.RunAsync();
