using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for a student session application
    /// </summary>
    public class StudentSessionApplicationDto
    {
        public int? Id { get; set; }

        public string Motivation { get; set; }

        public StudentSessionApplicationStatus Status { get; set; }

        public int StudentId { get; set; }

        public int CompanyId { get; set; }

        public bool Booked { get; set; }

        public string StudentFirstName { get; set; }

        public string StudentLastName { get; set; }

        public int? StudentYear { get; set; }

        public Programme? StudentProgramme { get; set; }
        
    }
}
