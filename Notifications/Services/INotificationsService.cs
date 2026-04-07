using Shared.Events;

namespace Notifications.Service.Services;

public interface INotificationsService
{
    Task ProcessInventoryUpdatedEventAsync(InventoryUpdatedEvent @event, CancellationToken ct = default);
}
