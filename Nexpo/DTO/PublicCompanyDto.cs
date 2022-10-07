using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class PublicCompanyDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public string DidYouKnow { get; set; }
        public string LogoUrl { get; set; }
        public string Website { get; set; }
    }
}
