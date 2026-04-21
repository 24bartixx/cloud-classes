using ClanGames.Infrastructure.Bus;
using MediatR;
using ClanGames.Application.Commands.PublishClanWarEnded;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRabbitMqMessaging(serviceName: "ClanWars.Api");
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssemblyContaining<PublishClanWarEndedCommand>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
