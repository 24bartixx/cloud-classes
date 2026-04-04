using Shared.Events;

namespace Clans.Service.Services;

public interface IClansService
{
    Task ProcessClanWarEndedEventAsync(ClanWarEndedEvent @event, CancellationToken ct = default);
}
