using Amazon.S3;
using Clans.Infrastructure.Bus;
using Clans.Service.Infrastructure.Persistence;
using Clans.Application.Commands.ProcessClanWarEnded;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddDefaultAWSOptions(ctx.Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        services.AddRabbitMqMessaging(serviceName: "Clans.Service");
        services.AddPersistence(ctx.Configuration);
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<ProcessClanWarEndedCommand>());
        services.AddHostedService<Clans.Service.Workers.ClansWorker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ClansDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();
