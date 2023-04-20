using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }

        // Either "company event", "lunch", "banquet"
        public EventType Type { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string Start { get; set; }
        [Required]
        public string End { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Host { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public int Capacity { get; set; }

        [JsonIgnore]
        public IEnumerable<Ticket> Tickets { get; set; }
        [NotMapped]
        public int TicketCount { get; set; }
    }

        public enum EventType
    {
        CompanyEvent,
        Lunch,
        Banquet
    }

}

