using System;
using System.Collections.Generic;

namespace Nexpo.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public NotificationType NotificationType { get; set; }
        public int? EventId { get; set; }
    }
}