using ClanGames.Application;
using ClanGames.Infrastructure.Bus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRabbitMqMessaging(serviceName: "ClanWars.Api");
builder.Services.AddScoped<IClanGamesService, ClanGamesService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
