using System;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CreateStudentSessionTimeslotDto
    { 
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public string Location { get; set; }
    }
}
