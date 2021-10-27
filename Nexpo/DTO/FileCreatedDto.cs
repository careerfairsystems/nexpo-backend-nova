using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class FileCreatedDto
    {
        [Required]
        public string Url { get; set; }
    }
}
