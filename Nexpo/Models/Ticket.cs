using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nexpo.Models
{
    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        [Required]
        public Guid Code { get; set; } = Guid.NewGuid();
        [Required]
        public bool PhotoOk { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
        public int UserId { get; set; }
        //TRAP
        [JsonIgnore]
        public User User { get; set; }
    }
}

