using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Nexpo.Models;

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
        public List<Degree> DesiredDegrees { get; set;}
        public List<Guild> DesiredGuilds { get; set;}
        public List<Position> Positions { get; set;}
        public List<Industry> Industries { get; set;}
    }
}
