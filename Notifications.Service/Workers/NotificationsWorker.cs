using TanksGame.Shared.Configuration;
using TanksGame.Shared.Events;
using TanksGame.Shared.Messaging;

namespace Notifications.Service.Workers;

public sealed class NotificationsWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly ILogger<NotificationsWorker> _logger;

    public NotificationsWorker(
        IMessageConsumer consumer,
        ILogger<NotificationsWorker> logger)
    {
        _consumer = consumer;
        _logger   = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Notifications.Service starting — consuming from queue '{Queue}'.",
            QueueNames.NotificationsInventoryUpdated);

        await _consumer.StartConsumingAsync<InventoryUpdatedEvent>(
            QueueNames.NotificationsInventoryUpdated,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(InventoryUpdatedEvent @event)
    {
        _logger.LogInformation(
            "Sending notification to Player '{PlayerName}' (Id={PlayerId}) — {ItemCount} item(s) granted.",
            @event.PlayerName, @event.PlayerId, @event.GrantedItems.Count);

        var itemsSummary = string.Join(", ",
            @event.GrantedItems.Select(i => $"{i.ItemName} x{i.Quantity}"));

        var notification = new NotificationRequestEvent
        {
            PlayerId   = @event.PlayerId,
            PlayerName = @event.PlayerName,
            Channel    = "in-game",
            Title      = "Clan War Rewards Received!",
            Body       = $"You received the following rewards: {itemsSummary}. " +
                         $"Check your garage to claim them!",
            ScheduledAtUtc = DateTime.UtcNow
        };

        _logger.LogInformation(
            "[{Channel}] → '{Title}' sent to '{PlayerName}'. Body: {Body}",
            notification.Channel, notification.Title,
            notification.PlayerName, notification.Body);

        await Task.CompletedTask;
    }
}
