using Clans.Infrastructure.Bus;
using Clans.Service.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Clans.Service");
        services.AddPersistence(ctx.Configuration);
        services.AddHostedService<Clans.Service.Workers.ClansWorker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ClansDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();
