using Shared.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Inventories.Service.Services;

public interface IInventoriesService
{
    Task UpsertInventoryAsync(InventoryUpdatedEvent @event, CancellationToken ct = default);
}
