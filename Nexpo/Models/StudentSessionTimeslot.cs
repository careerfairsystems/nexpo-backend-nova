using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a timeslot that a student can book a session in
    /// </summary>
    public class StudentSessionTimeslot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public string Location { get; set; }

        public int CompanyId { get; set; }

        [JsonIgnore]
        public Company Company { get; set; }
        
        public int? StudentId { get; set; }
    }
}

