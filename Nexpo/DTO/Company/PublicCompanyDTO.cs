using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for creating a new company
    /// </summary>
    public class PublicCompanyDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string DidYouKnow { get; set; }

        public string LogoUrl { get; set; }

        public string Website { get; set; }

        public List<int> DesiredDegrees { get; set;}

        public List<int> DesiredProgramme { get; set;}

        public List<int> Positions { get; set;}

        public List<int> Industries { get; set;}
        
        public string StudentSessionMotivation { get; set; }

    }
}
