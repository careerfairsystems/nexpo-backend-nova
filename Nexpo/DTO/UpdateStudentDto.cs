using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student
    /// </summary>
    public class UpdateStudentDto
    {
        public Programme? Programme { get; set; }

        public string LinkedIn { get; set; }

        public string MasterTitle { get; set; }
        
        public int? Year { get; set; }
    }
}
