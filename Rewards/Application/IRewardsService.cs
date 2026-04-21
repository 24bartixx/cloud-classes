using Shared.Events;

namespace Rewards.Application;

public interface IRewardsService
{
    RewardItemDto SelectRewardForPlayer(PlayerStatsDto player, IReadOnlyList<Rewards.Domain.Entities.Reward> rewards);
}
