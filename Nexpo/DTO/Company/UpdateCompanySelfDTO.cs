using System;
using System.Collections.Generic;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a company
    /// </summary>
    /// <remarks>
    /// This DTO is used when updating a company by a company representative
    /// </remarks>
    public class UpdateCompanySelfDTO
    {
        public string Description { get; set; }

        public string DidYouKnow { get; set; }
        
        public string Website { get; set; }

        public List<DateTime> DaysAtArkad { get; set;}
    }
}
