using Shared.Events;

namespace PlayerMatchesHistory.Service.Services;

public interface IPlayerMatchesHistoryService
{
    Task SavePlayerStatsAsync(ClanWarEndedEvent @event, CancellationToken ct = default);
}
