using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for finalizing a sign up
    /// </summary>
    public class FinalizeSignUpDTO
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Password { get; set; }
        

        public class FinalizeSignUpTokenDTO
        {
            public int UserId { get; set; }
        }
    }
}
