using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class UpdateStudentSessionLocationDto
    {
        [Required]
        public string Location { get; set; }
    }
}
