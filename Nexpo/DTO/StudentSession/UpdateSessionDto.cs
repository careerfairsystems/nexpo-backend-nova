using Nexpo.Models;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student session
    public class UpdateSessionDto
    {
        [Required]
        public StudentSessionApplicationStatus Status { get; set; }
    }
}
