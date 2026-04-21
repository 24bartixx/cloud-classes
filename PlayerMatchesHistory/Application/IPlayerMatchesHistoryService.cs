using Shared.Events;

namespace PlayerMatchesHistory.Application;

public interface IPlayerMatchesHistoryService
{
    Task SavePlayerStatsAsync(ClanWarEndedEvent @event, CancellationToken ct = default);
}
