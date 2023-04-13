using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a ticket
    /// </summary>
    public class UpdateTicketDto
    {
        [Required]
        public bool isConsumed { set; get; }
    }
}
