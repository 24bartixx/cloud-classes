using TanksGame.Shared.Messaging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Clans.Service");
        services.AddHostedService<Clans.Service.Workers.ClansWorker>();
    })
    .Build();

await host.RunAsync();
