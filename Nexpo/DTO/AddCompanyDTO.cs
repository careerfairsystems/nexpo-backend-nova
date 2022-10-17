using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nexpo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public List<int> DesiredGuilds { get; set;}
        public List<int> Positions { get; set;}

        public List<int> Industries { get; set;}

    }
}
