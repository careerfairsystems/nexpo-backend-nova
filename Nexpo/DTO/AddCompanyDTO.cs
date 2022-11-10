using System.Collections.Generic;

namespace Nexpo.DTO
{
    public class AddCompanyDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DidYouKnow { get; set; }
        public string Website { get; set; }

        public string LogoUrl { get; set; }

        public string HostName { get; set; }

        public string HostEmail { get; set; }

        public string HostPhone { get; set; }

        public List<int> DesiredDegrees { get; set;}

        public List<int> DesiredProgramme { get; set;}
        public List<int> Positions { get; set;}

        public List<int> Industries { get; set;}
        public string StudentSessionMotivation { get; set; }
    }
}
