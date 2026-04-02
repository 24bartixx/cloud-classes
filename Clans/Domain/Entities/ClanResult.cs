namespace Clans.Domain.Entities;

public record ClanResult
{
    public Guid ClanId { get; init; }
    public string ClanName { get; init; } = string.Empty;
    public int TotalScore { get; init; }
    public bool IsWinner { get; init; }
    public int NewRating { get; init; }
    public int RatingDelta { get; init; }
}
