using Nexpo.Models;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class TicketInfoDTO
    {
        [Required]
        public string userFirstName { get; set; }

        [Required]
        public string userLastName { get; set; }
        
        [Required]
        public Ticket ticket { get; set; }
    }
}
