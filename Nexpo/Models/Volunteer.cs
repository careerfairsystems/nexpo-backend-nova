using System;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a user of the Role student
    /// </summary>
    public class Volunteer
    {
        public int? Id { get; set; } = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

        public Programme? Programme { get; set; }

        public string ResumeEnUrl { get; set; }

        public string ResumeSvUrl { get; set; }

        public string LinkedIn { get; set; }

        public string MasterTitle { get; set; }

        public int? Year { get; set; }

        public int UserId { get; set; }
        
        //TRAP!
        [JsonIgnore]
        public User User { get; set; }
    }
}

