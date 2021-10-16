using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CompanyCompanyConnectionDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public int StudentId { get; set; }
    }
}

