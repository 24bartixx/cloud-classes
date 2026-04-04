namespace ClanGames.Domain.Entities;

public record ClanResult
{
    public Guid ClanId { get; init; }
    public int Score { get; init; }
    public int Placement { get; init; }
    public int TotalClans { get; init; }
}
