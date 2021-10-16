using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CreateCompanyConnectionDto
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
