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
        public StudentSessionApplicationStatus Status { get; set; } = StudentSessionApplicationStatus.NoResponse;
        public int StudentId { get; set; }
        [JsonIgnore]
        public Student Student { get; set; }
        public int CompanyId { get; set; }
        [JsonIgnore]
        public Company Company { get; set; }
        public bool Booked { get; set; } = false;
    }

    public enum StudentSessionApplicationStatus
    {
        NoResponse,
        Accepted,
        Declined
    }
}

