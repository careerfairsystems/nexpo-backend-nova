using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nexpo.Models
{
    public class StudentSession
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public StudentSessionStatus Status { get; set; } = StudentSessionStatus.NoResponse;

        public int StudentId { get; set; }
        [JsonIgnore]
        public Student Student { get; set; }
        [ForeignKey(nameof(StudentSessionTimeslot))]
        public int StudentSessionTimeslotId { get; set; }
        [JsonIgnore]
        public StudentSessionTimeslot StudentSessionTimeslot { get; set; }
        [ForeignKey(nameof(StudentSessionApplication))]
        public int StudentSessionApplicationId { get; set; }
        [JsonIgnore]
        public StudentSessionApplication StudentSessionApplication { get; set; }
    }

    public enum StudentSessionStatus
    {
        NoResponse,
        Accepted,
        Declined
    }
}

