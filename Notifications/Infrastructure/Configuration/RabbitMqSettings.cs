namespace Notifications.Infrastructure.Configuration;

public class RabbitMqSettings
{
    public string ConnectionUrl { get; set; } = string.Empty;
}

public static class QueueNames
{
    public const string NotificationsInventoryUpdated = "Notifications.InventoryUpdatedEvent";
}
