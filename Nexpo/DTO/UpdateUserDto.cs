using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNr { get; set; }
        public string FoodPreferences { get; set; }
        public string Password { get; set; }

        public string profilePictureUrl { get; set; }
    }
}
