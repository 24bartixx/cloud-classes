using TanksGame.Shared.Messaging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Notifications.Service");
        services.AddHostedService<Notifications.Service.Workers.NotificationsWorker>();
    })
    .Build();

await host.RunAsync();
