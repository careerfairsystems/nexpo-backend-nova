using System.ComponentModel.DataAnnotations;
using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for sending a ticket via mail
    /// </summary>
    public class SendTickerViaMailDTO
    {
        [Required]
        public string mail { get; set; }

        [Required]
        public string QRCode { get; set; }

        [Required]
        public int EventId { get; set; }
    }
}
