using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class EventFoodDto
    {
        [Required]
        public string Preference { get; set; }

        [Required]
        public int Count { get; set; }
    }
}