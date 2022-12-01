
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        public string Email { get; set; }
    }
}
