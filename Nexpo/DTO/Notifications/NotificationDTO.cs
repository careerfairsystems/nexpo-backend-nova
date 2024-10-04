using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Nexpo.Models;

namespace Nexpo.DTO.Notifications
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }

        public DateTime? ScheduledTime { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        public int? EventId { get; set; }

        [Required]
        public List<int> UserIds { get; set; }
    }
}
