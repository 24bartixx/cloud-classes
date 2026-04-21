using Shared.Events;

namespace Inventories.Application;

public interface IInventoriesService
{
    Task UpsertInventoryAsync(InventoryUpdatedEvent @event, CancellationToken ct = default);
}
