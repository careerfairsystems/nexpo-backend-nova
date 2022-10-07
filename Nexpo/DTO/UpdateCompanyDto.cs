using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class UpdateCompanyDto
    {
        public string Description { get; set; }
        public string DidYouKnow { get; set; }
        public string Website { get; set; }
        public string HostName { get; set; }
        public string HostEmail { get; set; }
        public string HostPhone { get; set; }
    }
}
