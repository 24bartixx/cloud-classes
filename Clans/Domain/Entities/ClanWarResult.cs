namespace Clans.Domain.Entities;

public class ClanWarResult
{
    public Guid ClanWarId { get; set; }
    public DateTime FinishDate { get; set; } = DateTime.UtcNow;
    public int TotalClans { get; set; }
}
