namespace Shared.Configuration;

public class RabbitMqSettings
{
    public string ConnectionUrl { get; set; } = "amqps://gzjxaaje:Ip8MJJlN8PSUGhqbm5aiVEDFKx69aGr-@cow.rmq2.cloudamqp.com/gzjxaaje";
}

public static class ExchangeNames
{
    public const string ClanWarEnded = "ClanWarEndedEvent";
}

public static class QueueNames
{
    public const string ClansWarEnded = "Clans.ClanWarEndedEvent";
    public const string PlayerMatchesHistoryEnded = "PlayerMatchesHistory.ClanWarEndedEvent";
    public const string RewardsWarEnded = "Rewards.ClanWarEndedEvent";
    public const string InventoriesRewardSelected = "Inventories.RewardSelectedEvent";
    public const string NotificationsInventoryUpdated = "Notifications.InventoryUpdatedEvent";
}
