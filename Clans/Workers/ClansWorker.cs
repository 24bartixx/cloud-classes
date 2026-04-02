using RabbitMQ.Client;
using Clans.Infrastructure.Configuration;
using Clans.Infrastructure.Bus;
using Shared.Events;

namespace Clans.Service.Workers;

public sealed class ClansWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly ILogger<ClansWorker> _logger;
    private readonly RabbitMqSettings _settings;

    public ClansWorker(
        IMessageConsumer consumer,
        ILogger<ClansWorker> logger,
        RabbitMqSettings settings)
    {
        _consumer = consumer;
        _logger   = logger;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Clans.Service starting — binding queue '{Queue}' to exchange '{Exchange}'.",
            QueueNames.ClansWarEnded, ExchangeNames.ClanWarEnded);

        BindQueueToFanoutExchange(
            QueueNames.ClansWarEnded,
            ExchangeNames.ClanWarEnded);

        await _consumer.StartConsumingAsync<ClanWarEndedEvent>(
            QueueNames.ClansWarEnded,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(ClanWarEndedEvent @event)
    {
        _logger.LogInformation(
            "Processing ClanWarEndedEvent for ClanWarId={ClanWarId} with {Count} clan results.",
            @event.ClanWarId, @event.ClanResults.Count);

        foreach (var clan in @event.ClanResults)
        {
            // Internal processing logic here
        }

        await Task.CompletedTask;
    }

    private void BindQueueToFanoutExchange(string queue, string exchange)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_settings.ConnectionUrl) };
        using var conn    = factory.CreateConnection();
        using var channel = conn.CreateModel();

        channel.ExchangeDeclare(exchange, ExchangeType.Fanout, durable: true);
        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue: queue, exchange: exchange, routingKey: string.Empty);

        _logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}'.", queue, exchange);
    }
}
