using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }

        public class ResetPasswordTokenDto
        {
            public int UserId { get; set; }
        }
    }
}
