using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CreateStudentSessionApplicationDto
    {
        [Required]
        public int CompanyId { get; set; }
        public string Motivation { get; set; }
    }
}
