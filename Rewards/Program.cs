using Rewards.Infrastructure.Bus;
using Rewards.Service.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Rewards.Service");
        services.AddPersistence(ctx.Configuration);
        services.AddScoped<Rewards.Application.IRewardsService, Rewards.Application.RewardsService>();
        services.AddHostedService<Rewards.Service.Workers.RewardsWorker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RewardsDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();
