using Nexpo.Models;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student session
    public class UpdateSessionDTO
    {
        [Required]
        public StudentSessionApplicationStatus Status { get; set; }
    }
}
