
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CreateFAQDTO
    {

        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }


    }
}
