namespace Clans.Domain.Entities;

public class ClanResult
{
    public Guid ClanResultId { get; set; }
    public Guid ClanId { get; set; }
    public Guid ClanWarId { get; set; }
    public int Placement { get; set; }
    public int Score { get; set; }
}
