using System;
using Nexpo.Models;

namespace Nexpo.DTO.Notifications
{
    public class AddNotificationDTO
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public NotificationType NotificationType { get; set; }
        public int? EventId { get; set; }
    }
}