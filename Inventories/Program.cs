using Shared.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Inventories.Service");
        services.AddHostedService<Inventories.Service.Workers.InventoriesWorker>();
    })
    .Build();

await host.RunAsync();
