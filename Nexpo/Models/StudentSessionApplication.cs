using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    public class StudentSessionApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string Motivation { get; set; }
        public StudentSessionApplicationStatus Status { get; set; } = StudentSessionApplicationStatus.NoResponse;
        public int StudentId { get; set; }
        //TRAP
        [JsonIgnore]
        public Student Student { get; set; }
        public int CompanyId { get; set; }
        //TRAP
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

