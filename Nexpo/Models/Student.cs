using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public Guild? Guild { get; set; }
        public string ResumeEnUrl { get; set; }
        public string ResumeSvUrl { get; set; }
        public string LinkedIn { get; set; }
        public string MasterTitle { get; set; }
        public int? Year { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
    public enum Guild
    {
        A, D, E, F, I, ING, K, M, V
    }
}

