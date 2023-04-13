using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student session location
    /// </summary>
    public class UpdateStudentSessionLocationDto
    {
        [Required]
        public string Location { get; set; }
    }
}
