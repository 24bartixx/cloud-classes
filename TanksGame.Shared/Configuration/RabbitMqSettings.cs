namespace TanksGame.Shared.Configuration;

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
    public const string ClansWarEnded = "clans.ClanWarEndedEvent";
    public const string PlayerMatchesHistoryEnded = "player-matches-history.ClanWarEndedEvent";
    public const string RewardsWarEnded = "rewards.ClanWarEndedEvent";
    public const string InventoriesRewardSelected = "inventories.RewardSelectedEvent";
    public const string NotificationsInventoryUpdated = "notifications.InventoryUpdatedEvent";
}
