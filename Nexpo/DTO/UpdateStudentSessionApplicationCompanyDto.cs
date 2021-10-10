using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class UpdateStudentSessionApplicationCompanyDto
    {
        [Required]
        public int Rating { get; set; }
    }
}
