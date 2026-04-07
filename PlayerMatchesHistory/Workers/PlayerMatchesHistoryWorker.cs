using RabbitMQ.Client;
using PlayerMatchesHistory.Infrastructure.Configuration;
using PlayerMatchesHistory.Infrastructure.Bus;
using Shared.Events;

namespace PlayerMatchesHistory.Service.Workers;

public sealed class PlayerMatchesHistoryWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly ILogger<PlayerMatchesHistoryWorker> _logger;
    private readonly RabbitMqSettings _settings;
    private readonly PlayerMatchesHistory.Service.Services.IPlayerMatchesHistoryService _playerMatchesHistoryService;

    public PlayerMatchesHistoryWorker(
        IMessageConsumer consumer,
        ILogger<PlayerMatchesHistoryWorker> logger,
        RabbitMqSettings settings,
        PlayerMatchesHistory.Service.Services.IPlayerMatchesHistoryService playerMatchesHistoryService)
    {
        _consumer = consumer;
        _logger   = logger;
        _settings = settings;
        _playerMatchesHistoryService = playerMatchesHistoryService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "PlayerMatchesHistory.Service starting — binding queue '{Queue}' to exchange '{Exchange}'.",
            QueueNames.PlayerMatchesHistoryEnded, ExchangeNames.ClanWarEnded);

        BindQueueToFanoutExchange(
            QueueNames.PlayerMatchesHistoryEnded,
            ExchangeNames.ClanWarEnded);

        await _consumer.StartConsumingAsync<ClanWarEndedEvent>(
            QueueNames.PlayerMatchesHistoryEnded,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(ClanWarEndedEvent @event)
    {
        _logger.LogInformation(
            "Storing match history for ClanWarId={ClanWarId} — {Count} player records.",
            @event.ClanWarId, @event.PlayerStats.Count);

        await _playerMatchesHistoryService.SavePlayerStatsAsync(@event);
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
