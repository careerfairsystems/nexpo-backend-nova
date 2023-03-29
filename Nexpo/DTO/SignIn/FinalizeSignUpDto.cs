using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class FinalizeSignUpDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }

        public class FinalizeSignUpTokenDto
        {
            public int UserId { get; set; }
        }
    }
}
