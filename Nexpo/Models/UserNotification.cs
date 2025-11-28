using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a notification topic
    /// </summary>
    public class UserNotification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [JsonIgnore]
        public User User { get; set; }
        [Required]
        public int NotificationId { get; set; }
        [Required]
        [JsonIgnore]
        public Notification Notification { get; set; }

    }
}

