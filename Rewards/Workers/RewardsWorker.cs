using RabbitMQ.Client;
using Rewards.Infrastructure.Configuration;
using Rewards.Infrastructure.Bus;
using Shared.Events;

namespace Rewards.Service.Workers;

public sealed class RewardsWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<RewardsWorker> _logger;
    private readonly RabbitMqSettings _settings;

    public RewardsWorker(
        IMessageConsumer consumer,
        IMessagePublisher publisher,
        ILogger<RewardsWorker> logger,
        RabbitMqSettings settings)
    {
        _consumer  = consumer;
        _publisher = publisher;
        _logger    = logger;
        _settings  = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Rewards.Service starting — binding queue '{Queue}' to exchange '{Exchange}'.",
            QueueNames.RewardsWarEnded, ExchangeNames.ClanWarEnded);

        BindQueueToFanoutExchange(
            QueueNames.RewardsWarEnded,
            ExchangeNames.ClanWarEnded);

        await _consumer.StartConsumingAsync<ClanWarEndedEvent>(
            QueueNames.RewardsWarEnded,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(ClanWarEndedEvent @event)
    {
        _logger.LogInformation(
            "Selecting rewards for ClanWarId={ClanWarId} — {Count} players.",
            @event.ClanWarId, @event.PlayerStats.Count);

        foreach (var player in @event.PlayerStats)
        {
            var selectedReward = new RewardItemDto
            {
                ItemId   = Guid.NewGuid(),
                ItemName = "Standard War Chest",
                ItemType = "Consumable",
                Quantity = 1
            };

            var rewardEvent = new RewardSelectedEvent
            {
                ClanWarId  = @event.ClanWarId,
                PlayerId   = player.PlayerId,
                PlayerName = player.PlayerName,
                Reward     = selectedReward
            };

            _publisher.PublishToQueue(QueueNames.InventoriesRewardSelected, rewardEvent);
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
