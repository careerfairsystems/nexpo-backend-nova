using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a ticket for an event, lunch lecture, banquet
    /// </summary>
    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        public Guid Code { get; set; } = Guid.NewGuid();

        [Required]
        public bool PhotoOk { get; set; }

        public bool isConsumed { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }

        public int UserId { get; set; }

        public DateTime TakeAway { get; set; }
        
        //TRAP
        [JsonIgnore]
        public User User { get; set; }
    }
}

