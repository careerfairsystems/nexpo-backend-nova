using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for creating a ticket
    /// </summary>
    /// <remarks>
    /// This DTO is used when creating a ticket by a user
    /// </remarks>
    public class CreateTicketDto
    {
        [Required]
        public int EventId { get; set; }
        
        [Required]
        public bool PhotoOk { get; set; }
    }
}
