
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    public class UpdateContactDTO
    {

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string RoleInArkad { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
    }
}
