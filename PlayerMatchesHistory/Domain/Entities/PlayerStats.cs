namespace PlayerMatchesHistory.Domain.Entities;

public record PlayerStats
{
    public Guid PlayerId { get; init; }
    public string PlayerName { get; init; } = string.Empty;
    public Guid ClanId { get; init; }
    public int Kills { get; init; }
    public int Deaths { get; init; }
    public int DamageDealt { get; init; }
    public int DamageReceived { get; init; }
    public bool Survived { get; init; }
    public int ExperienceEarned { get; init; }
}
