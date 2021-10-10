using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class CreateTicketDto
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public bool PhotoOk { get; set; }
    }
}
