using System.ComponentModel.DataAnnotations;

namespace Notifications.Domain.Entities;

public class Notification
{
    [Key]
    public Guid NotificationId { get; set; }
    
    public Guid PlayerId { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; }
}
