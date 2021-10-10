using Nexpo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class UpdateSessionDto
    {
        [Required]
        public StudentSessionStatus Status { get; set; }
    }
}
