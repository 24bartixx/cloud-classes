using Notifications.Domain.Entities;
using Notifications.Infrastructure.Persistence;
using Shared.Events;

namespace Notifications.Application;

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
        var reward = @event.Reward;
        string itemsSummary = $"Credits: {reward.Credits}, Experience: {reward.Experience}";
        if (reward.Tanks.Count > 0)
            itemsSummary += $", Tanks: {string.Join(", ", reward.Tanks)}";

        _logger.LogInformation(
            "Processing notification for Player '{PlayerName}' (Id={PlayerId}) — Rewards: {ItemsSummary}",
            @event.PlayerName, @event.PlayerId, itemsSummary);

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                PlayerId = @event.PlayerId,
                Message = $"{itemsSummary} (Player: {@event.PlayerName})",
                SentAt = DateTime.UtcNow
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync(ct);
        }
    }
}
