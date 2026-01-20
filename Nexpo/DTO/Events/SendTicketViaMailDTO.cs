using System.ComponentModel.DataAnnotations;
using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for sending a ticket via mail
    /// </summary>
    public class SendTicketViaMailDTO
    {
        [Required]
        public string mail { get; set; }

        [Required]
        public int eventId { get; set; }

        [Required]
        public int numberOfTickets { get; set; }

        public string appearAt  { get; set; }

    }
}
