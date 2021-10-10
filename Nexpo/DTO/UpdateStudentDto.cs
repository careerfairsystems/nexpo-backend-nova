using Nexpo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class UpdateStudentDto
    {
        public Guild? Guild { get; set; }
        public string LinkedIn { get; set; }
        public string MasterTitle { get; set; }
        public int? Year { get; set; }
    }
}
