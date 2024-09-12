using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a notification topic
    /// </summary>
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime ScheduledTime { get; set; }
        [Required]
        public NotificationType Type { get; set; }
        [Required]
        public int? EventId { get; set; }
   



        public enum NotificationType
        {
            General,
            EventReminder,
            InternalMessage,
        }

    }
}

