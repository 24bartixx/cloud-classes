using Shared.Events;
using System.Collections.Generic;
using System.Linq;

namespace Rewards.Service.Services;

public class RewardsService : IRewardsService
{
    public RewardItemDto SelectRewardForPlayer(PlayerStatsDto player, IReadOnlyList<Rewards.Domain.Entities.Reward> rewards)
    {
        var matched = rewards.Where(r =>
            (!r.MinKills.HasValue || player.Kills >= r.MinKills) &&
            (!r.MaxDeaths.HasValue || player.Deaths <= r.MaxDeaths) &&
            (!r.MinDamageDealt.HasValue || player.DamageDealt >= r.MinDamageDealt) &&
            (!r.MaxDamageReceived.HasValue || player.DamageReceived <= r.MaxDamageReceived) &&
            (!r.MinSurvived.HasValue || (player.Survived ? 1 : 0) >= r.MinSurvived) &&
            (!r.MinExperienceEarned.HasValue || player.ExperienceEarned >= r.MinExperienceEarned)
        ).ToList();

        int totalCredits = matched.Sum(r => r.Credits);
        int totalExp = matched.Sum(r => r.Experience);
        var tanks = matched.Where(r => !string.IsNullOrWhiteSpace(r.TankId)).Select(r => r.TankId!).Distinct().ToList();

        return new RewardItemDto
        {
            Credits = totalCredits,
            Experience = totalExp,
            Tanks = tanks
        };
    }
}
