using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student session location
    /// </summary>
    public class UpdateStudentSessionLocationDTO
    {
        [Required]
        public string Location { get; set; }
    }
}
