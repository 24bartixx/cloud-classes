using Inventories.Infrastructure.Configuration;
using Inventories.Infrastructure.Bus;
using Shared.Events;

namespace Inventories.Service.Workers;

public sealed class InventoriesWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<InventoriesWorker> _logger;
    private readonly Application.IInventoriesService _inventoriesService;

    public InventoriesWorker(
        IMessageConsumer consumer,
        IMessagePublisher publisher,
        ILogger<InventoriesWorker> logger,
        Inventories.Application.IInventoriesService inventoriesService)
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
        var reward = @event.Reward;
        string rewardSummary = $"Credits: {reward.Credits}, Experience: {reward.Experience}";
        if (reward.Tanks.Count > 0)
            rewardSummary += $", Tanks: {string.Join(", ", reward.Tanks)}";

        _logger.LogInformation(
            "Granting reward [{RewardSummary}] to Player '{PlayerName}' (Id={PlayerId}).",
            rewardSummary,
            @event.PlayerName, @event.PlayerId);

        var inventoryUpdatedEvent = new InventoryUpdatedEvent
        {
            PlayerId     = @event.PlayerId,
            PlayerName   = @event.PlayerName,
            Reward       = reward,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _inventoriesService.UpsertInventoryAsync(inventoryUpdatedEvent);

        _publisher.PublishToQueue(
            QueueNames.NotificationsInventoryUpdated,
            inventoryUpdatedEvent);
    }
}
