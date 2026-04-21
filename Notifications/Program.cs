using Notifications.Infrastructure.Bus;
using Notifications.Infrastructure.Persistence;
using Notifications.Application.Commands.SendNotification;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddRabbitMqMessaging(serviceName: "Notifications.Service");
        services.AddPersistence(ctx.Configuration);
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<SendNotificationCommand>());
        services.AddHostedService<Notifications.Service.Workers.NotificationsWorker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();
