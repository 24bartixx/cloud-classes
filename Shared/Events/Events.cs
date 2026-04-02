namespace Shared.Events;

public record ClanWarEndedEvent
{
    public Guid ClanWarId { get; init; } = Guid.NewGuid();
    public DateTime EndedAtUtc { get; init; } = DateTime.UtcNow;
    public IReadOnlyList<ClanResult> ClanResults { get; init; } = [];
    public IReadOnlyList<PlayerStats> PlayerStats { get; init; } = [];
}

public record ClanResult
{
    public Guid ClanId { get; init; }
    public string ClanName { get; init; } = string.Empty;
    public int TotalScore { get; init; }
    public bool IsWinner { get; init; }
    public int NewRating { get; init; }
    public int RatingDelta { get; init; }
}

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

public record RewardSelectedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public Guid ClanWarId { get; init; }
    public Guid PlayerId { get; init; }
    public string PlayerName { get; init; } = string.Empty;
    public RewardItem Reward { get; init; } = new();
}

public record RewardItem
{
    public Guid ItemId { get; init; } = Guid.NewGuid();
    public string ItemName { get; init; } = string.Empty;
    public string ItemType { get; init; } = string.Empty;
    public int Quantity { get; init; } = 1;
}

public record InventoryUpdatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public Guid PlayerId { get; init; }
    public string PlayerName { get; init; } = string.Empty;
    public IReadOnlyList<RewardItem> GrantedItems { get; init; } = [];
    public DateTime UpdatedAtUtc { get; init; } = DateTime.UtcNow;
}

public record NotificationRequestEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public Guid PlayerId { get; init; }
    public string PlayerName { get; init; } = string.Empty;
    public string Channel { get; init; } = "in-game";
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public DateTime ScheduledAtUtc { get; init; } = DateTime.UtcNow;
}
