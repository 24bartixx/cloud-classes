using Inventories.Infrastructure.Configuration;
using Inventories.Infrastructure.Bus;
using Shared.Events;

namespace Inventories.Service.Workers;

public sealed class InventoriesWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<InventoriesWorker> _logger;

    public InventoriesWorker(
        IMessageConsumer consumer,
        IMessagePublisher publisher,
        ILogger<InventoriesWorker> logger)
    {
        _consumer  = consumer;
        _publisher = publisher;
        _logger    = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Inventories.Service starting — consuming from queue '{Queue}'.",
            QueueNames.InventoriesRewardSelected);

        await _consumer.StartConsumingAsync<RewardSelectedEvent>(
            QueueNames.InventoriesRewardSelected,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(RewardSelectedEvent @event)
    {
        _logger.LogInformation(
            "Granting reward '{RewardName}' (x{Qty}) to Player '{PlayerName}' (Id={PlayerId}).",
            @event.Reward.ItemName, @event.Reward.Quantity,
            @event.PlayerName, @event.PlayerId);

        var inventoryUpdatedEvent = new InventoryUpdatedEvent
        {
            PlayerId     = @event.PlayerId,
            PlayerName   = @event.PlayerName,
            GrantedItems = [@event.Reward],
            UpdatedAtUtc = DateTime.UtcNow
        };

        _publisher.PublishToQueue(
            QueueNames.NotificationsInventoryUpdated,
            inventoryUpdatedEvent);

        await Task.CompletedTask;
    }
}
