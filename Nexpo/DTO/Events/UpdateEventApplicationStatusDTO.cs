using Nexpo.Models;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student session
    public class UpdateEventApplicationStatusDTO
    {
        [Required]
        public EventApplicationStatus Status { get; set; }
    }
}
