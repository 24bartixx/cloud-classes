namespace Rewards.Domain.Entities;

public class Reward
{
    public Guid RewardId { get; set; }
    public int? MinKills { get; set; }
    public int? MaxDeaths { get; set; }
    public int? MinDamageDealt { get; set; }
    public int? MaxDamageReceived { get; set; }
    public int? MinSurvived { get; set; }
    public int? MinExperienceEarned { get; set; }
    public string? TankId { get; set; }
    public int Experience { get; set; } = 0;
    public int Credits { get; set; } = 0;
}
