using Amazon.S3;
using Clans.Infrastructure.Bus;
using Clans.Service.Infrastructure.Persistence;
using Clans.Application.Commands.ProcessClanWarEnded;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddRabbitMqMessaging(serviceName: "Clans.Service");
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<ProcessClanWarEndedCommand>());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<Clans.Service.Workers.ClansWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ClansDbContext>();
    context.Database.EnsureCreated();
}

await app.RunAsync();
