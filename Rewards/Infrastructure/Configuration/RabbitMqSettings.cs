namespace Rewards.Infrastructure.Configuration;

public class RabbitMqSettings
{
    public string ConnectionUrl { get; set; } = string.Empty;
}

public static class ExchangeNames
{
    public const string ClanWarEnded = "ClanWarEndedEvent";
}

public static class QueueNames
{
    public const string RewardsWarEnded          = "Rewards.ClanWarEndedEvent";
    public const string InventoriesRewardSelected = "Inventories.RewardSelectedEvent";
}
