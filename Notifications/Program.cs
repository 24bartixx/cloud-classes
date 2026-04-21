using Notifications.Infrastructure.Bus;
using Notifications.Infrastructure.Persistence;
using Notifications.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Notifications.Service");
        services.AddPersistence(ctx.Configuration);
        services.AddSingleton<INotificationsService, NotificationsService>();
        services.AddHostedService<Notifications.Service.Workers.NotificationsWorker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();
