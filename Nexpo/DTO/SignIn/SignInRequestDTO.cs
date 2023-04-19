using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class SignInRequestDto
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
