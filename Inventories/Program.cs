using Amazon.DynamoDBv2;
using Inventories.Infrastructure.Bus;
using Inventories.Application.Commands.UpdateInventory;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddDefaultAWSOptions(ctx.Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddRabbitMqMessaging(ctx.Configuration, serviceName: "Inventories.Service");
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<UpdateInventoryCommand>());
        services.AddHostedService<Inventories.Service.Workers.InventoriesWorker>();
    })
    .Build();

await host.RunAsync();
