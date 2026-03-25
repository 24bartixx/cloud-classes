using TanksGame.Shared.Messaging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Rewards.Service");
        services.AddHostedService<Rewards.Service.Workers.RewardsWorker>();
    })
    .Build();

await host.RunAsync();
