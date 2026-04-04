namespace Clans.Domain.Entities;

public class Clan
{
    public Guid ClanId { get; set; }
    public string ClanName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
