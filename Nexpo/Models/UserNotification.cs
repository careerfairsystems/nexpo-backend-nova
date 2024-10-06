using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    public class UserNotification
    {
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public int NotificationId { get; set; }

        [JsonIgnore]
        public Notification Notification { get; set; }
    }
}