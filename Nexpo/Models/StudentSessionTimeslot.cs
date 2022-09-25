using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nexpo.Models
{
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

        [JsonIgnore]
        public StudentSessionApplication StudentSessionApplication { get; set; }
        public bool booked { get; set; } = false;
    }
}

