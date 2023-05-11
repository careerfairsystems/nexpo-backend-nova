using System;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a ticket
    /// </summary>
    public class UpdateTicketDTO
    {
        [Required]
        public bool isConsumed { set; get; }

        public DateTime TakeAway { get; set; }
        public DateTime TakeAwayTime { get; set; }
    }
}
