
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class SignUpUserDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
    }
}
