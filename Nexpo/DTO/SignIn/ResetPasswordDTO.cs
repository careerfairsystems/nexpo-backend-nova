using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for resetting a password
    /// </summary>
    public class ResetPasswordDTO
    {
        [Required]
        public string Token { get; set; }
        
        [Required]
        public string Password { get; set; }

        public class ResetPasswordTokenDTO
        {
            public int UserId { get; set; }
        }
    }
}
