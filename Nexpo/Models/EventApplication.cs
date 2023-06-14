using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a student's application to a company event
    /// </summary>
    public class EventApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        
        public string Motivation { get; set; }

        public EventApplicationStatus Status { get; set; } = EventApplicationStatus.NoResponse;

        public int StudentId { get; set; }

        public int EventId { get; set; }

        public int CompanyId { get; set; }

        //TRAP
        [JsonIgnore]
        public Student Student { get; set; }

        //TRAP
        [JsonIgnore]
        public Event Event { get; set; }

        public Company Company { get; set; }

        public bool Booked { get; set; } = false;
    }

    public enum EventApplicationStatus
    {
        NoResponse,
        Accepted,
        Declined
    }
}

