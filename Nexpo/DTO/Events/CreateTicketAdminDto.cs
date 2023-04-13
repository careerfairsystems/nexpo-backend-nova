
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CreateTicketAdminDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public bool PhotoOk { get; set; }
        
        [Required]
        public int UserId { get; set; }
    }
}
