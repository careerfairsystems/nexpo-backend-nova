using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nexpo.Models
{
    public class StudentSessionApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string Motivation { get; set; }
        public int Rating { get; set; } = 0;

        public int StudentId { get; set; }
        [JsonIgnore]
        public Student Student { get; set; }
        public int CompanyId { get; set; }
        [JsonIgnore]
        public Company Company { get; set; }
        [ForeignKey(nameof(StudentSession))]
        public int? StudentSessionId { get; set; }
        [JsonIgnore]
        public StudentSession StudentSession { get; set; }
    }
}

