using System;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for creating a student session timeslot
    /// </summary>
    public class CreateStudentSessionTimeslotAdminDTO
    { 
        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public int CompanyId { get; set; }
        
        public string Location { get; set; }
    }
}
