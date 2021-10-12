using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
