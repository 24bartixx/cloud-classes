namespace Rewards.Domain.Entities;

public record RewardItem
{
    public Guid ItemId { get; init; } = Guid.NewGuid();
    public string ItemName { get; init; } = string.Empty;
    public string ItemType { get; init; } = string.Empty;
    public int Quantity { get; init; } = 1;
}
