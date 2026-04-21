using Shared.Events;

namespace Clans.Application;

public interface IClansService
{
    Task ProcessClanWarEndedEventAsync(ClanWarEndedEvent @event, CancellationToken ct = default);
    Task ProcessFileMessageEventAsync(FileMessageEvent @event, CancellationToken ct = default);
}
