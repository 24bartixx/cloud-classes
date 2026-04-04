using Notifications.Infrastructure.Bus;
using Notifications.Service.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Notifications.Service");
        services.AddPersistence(ctx.Configuration);
        services.AddHostedService<Notifications.Service.Workers.NotificationsWorker>();
    })
    .Build();

await host.RunAsync();
