using Nexpo.Models;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class UpdateSessionDto
    {
        [Required]
        public StudentSessionApplicationStatus Status { get; set; }
    }
}
