using MediatR;
using Notifications.Domain.Entities;
using Notifications.Infrastructure.Persistence;

namespace Notifications.Application.Commands.SendNotification;

public sealed class SendNotificationCommandHandler(
    NotificationsDbContext dbContext,
    ILogger<SendNotificationCommandHandler> logger) : IRequestHandler<SendNotificationCommand, Guid>
{
    private readonly NotificationsDbContext _dbContext = dbContext;
    private readonly ILogger<SendNotificationCommandHandler> _logger = logger;

    public async Task<Guid> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var reward = request.EventData.Reward;

        var itemsSummary = $"Credits: {reward.Credits}, Experience: {reward.Experience}";
        if (reward.Tanks.Count > 0)
        {
            itemsSummary += $", Tanks: {string.Join(", ", reward.Tanks)}";
        }

        _logger.LogInformation(
            "Processing notification for Player '{PlayerName}' (Id={PlayerId}) - Rewards: {ItemsSummary}",
            request.EventData.PlayerName,
            request.EventData.PlayerId,
            itemsSummary);

        var notification = new Notification
        {
            NotificationId = Guid.NewGuid(),
            PlayerId = request.EventData.PlayerId,
            Message = $"{itemsSummary} (Player: {request.EventData.PlayerName})",
            SentAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Send notification

        return notification.NotificationId;
    }
}