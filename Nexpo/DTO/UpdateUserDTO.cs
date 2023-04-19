using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a user
    /// </summary>
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNr { get; set; }

        public string FoodPreferences { get; set; }
        
        public string Password { get; set; }
        public string profilePictureUrl { get; set; }
        public Role? Role { get; set; }
    }
}
