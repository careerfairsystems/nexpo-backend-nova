using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public NotificationType NotificationType { get; set; }
        public int? EventId { get; set; }
        [JsonIgnore]
        public ICollection<UserNotification> UserNotifications { get; set; }
    }
}