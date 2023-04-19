
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for inviting a representative to become a user
    /// </summary>
    public class InviteRepresentativeDto
    {
        [Required]
        public int CompanyId { get; set; }
        
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }

    }
}
