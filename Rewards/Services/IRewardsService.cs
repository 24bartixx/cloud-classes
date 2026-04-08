using Shared.Events;
using System.Collections.Generic;

namespace Rewards.Service.Services;

public interface IRewardsService
{
    RewardItemDto SelectRewardForPlayer(PlayerStatsDto player, IReadOnlyList<Rewards.Domain.Entities.Reward> rewards);
}
