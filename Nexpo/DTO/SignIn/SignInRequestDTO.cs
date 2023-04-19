using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class SignInRequestDTO
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
