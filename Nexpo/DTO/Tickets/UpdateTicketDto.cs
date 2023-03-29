using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class UpdateTicketDto
    {
        [Required]
        public bool isConsumed { set; get; }
    }
}
