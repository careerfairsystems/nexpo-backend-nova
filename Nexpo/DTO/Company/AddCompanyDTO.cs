using System.Collections.Generic;
using System;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for adding a company to the database
    /// </summary>
    public class AddCompanyDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string DidYouKnow { get; set; }

        public string Website { get; set; }

        public string LogoUrl { get; set; }

        public string HostName { get; set; }

        public string HostEmail { get; set; }

        public string HostPhone { get; set; }

        public List<DateTime> DaysAtArkad { get; set;}

        public List<int> DesiredDegrees { get; set;}

        public List<int> DesiredProgramme { get; set;}

        public List<int> Positions { get; set;}

        public List<int> Industries { get; set;}

        public string StudentSessionMotivation { get; set; }
        
    }
}
