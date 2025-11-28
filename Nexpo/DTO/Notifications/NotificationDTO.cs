using System;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class NotificationDTO
    {   
        [Required]
        public string Title { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string Message { get; set; }


        
    }
}