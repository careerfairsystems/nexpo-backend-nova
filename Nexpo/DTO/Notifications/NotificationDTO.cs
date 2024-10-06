using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Nexpo.Models;

namespace Nexpo.DTO.Notifications
{
    public class NotificationDTO
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
