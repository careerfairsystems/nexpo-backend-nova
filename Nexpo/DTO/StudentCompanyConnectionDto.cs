using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class StudentCompanyConnectionDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
    }
}
