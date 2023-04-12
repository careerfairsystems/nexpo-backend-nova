using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CreateTicketDto
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public bool PhotoOk { get; set; }
    }
}
