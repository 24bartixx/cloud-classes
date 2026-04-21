using Shared.Events;

namespace Notifications.Application;

public interface INotificationsService
{
    Task ProcessInventoryUpdatedEventAsync(InventoryUpdatedEvent @event, CancellationToken ct = default);
}
