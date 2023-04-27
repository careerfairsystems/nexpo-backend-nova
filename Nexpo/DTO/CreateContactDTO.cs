
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class CreateContactDTO
    {   
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string RoleInArkad { get; set; } 

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        
    }
}
