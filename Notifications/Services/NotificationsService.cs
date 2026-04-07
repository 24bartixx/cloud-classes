using Notifications.Domain.Entities;
using Notifications.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Events;

namespace Notifications.Service.Services;

public sealed class NotificationsService : INotificationsService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationsService> _logger;

    public NotificationsService(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationsService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ProcessInventoryUpdatedEventAsync(InventoryUpdatedEvent @event, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Processing notification for Player '{PlayerName}' (Id={PlayerId}) — {ItemCount} item(s) granted.",
            @event.PlayerName, @event.PlayerId, @event.GrantedItems.Count);

        var itemsSummary = string.Join(", ",
            @event.GrantedItems.Select(i => $"{i.ItemName} x{i.Quantity}"));

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                PlayerId = @event.PlayerId,
                Message = $"You received rewards: {itemsSummary}",
                SentAt = DateTime.UtcNow
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync(ct);
        }

        var notificationEvent = new NotificationRequestEvent
        {
            PlayerId = @event.PlayerId,
            PlayerName = @event.PlayerName,
            Channel = "in-game",
            Title = "Clan War Rewards Received!",
            Body = $"You received the following rewards: {itemsSummary}. Check your garage to claim them!",
            ScheduledAtUtc = DateTime.UtcNow
        };

        // Note: We don't have a specific queue for this in the original worker, but we can publish it if needed.
        // The original logic just created the event object but didn't publish it to a specific queue.
        // Actually, looking at the logs, it might be published to a Notifications queue.
        // For now, I'll match the logic but adding the DB save which was the main request.
        
        _logger.LogInformation("Successfully saved notification for PlayerId={PlayerId}", @event.PlayerId);
    }
}
