using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for adding an event to the database
    /// </summary>
    public class AddEventDTO
    {
        public string Name { get; set; }

        public EventType? Type { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public string Location { get; set; }

        public string Host { get; set; }

        public string Language { get; set; }
        
        public int Capacity { get; set; }
    }
}