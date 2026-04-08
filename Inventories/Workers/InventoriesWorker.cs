using Inventories.Infrastructure.Configuration;
using Inventories.Infrastructure.Bus;
using Shared.Events;

namespace Inventories.Service.Workers;

public sealed class InventoriesWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<InventoriesWorker> _logger;
    private readonly Inventories.Service.Services.IInventoriesService _inventoriesService;

    public InventoriesWorker(
        IMessageConsumer consumer,
        IMessagePublisher publisher,
        ILogger<InventoriesWorker> logger,
        Inventories.Service.Services.IInventoriesService inventoriesService)
    {
        _consumer  = consumer;
        _publisher = publisher;
        _logger    = logger;
        _inventoriesService = inventoriesService;
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

        await _inventoriesService.UpsertInventoryAsync(inventoryUpdatedEvent);

        _publisher.PublishToQueue(
            QueueNames.NotificationsInventoryUpdated,
            inventoryUpdatedEvent);
    }
}
