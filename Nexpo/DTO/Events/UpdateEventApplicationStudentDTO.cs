
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating an event application
    /// </summary>
    public class UpdateEventApplicationStudentDTO
    {
        [Required]
        public string Motivation { get; set; }
    }
}
