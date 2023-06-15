using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for an event application. Dont mix this up with events. It is an application for an event, not an event.
    /// </summary>
    public class EventApplicationDTO
    {
        public int? Id { get; set; }

        public string Motivation { get; set; }

        public EventApplicationStatus Status { get; set; }

        public int StudentId { get; set; }

        public int EventId { get; set; }

        public bool Booked { get; set; }

        public string StudentFirstName { get; set; }

        public string StudentLastName { get; set; }

        public int? StudentYear { get; set; }

        public Programme? StudentProgramme { get; set; }
        
    }
}
