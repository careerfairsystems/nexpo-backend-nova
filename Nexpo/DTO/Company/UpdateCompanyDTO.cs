
namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a company
    /// </summary>
    /// <remarks>
    /// This DTO is used when updating a company by a admin
    /// </remarks>
    public class UpdateCompanyDto
    {
        public string Description { get; set; }

        public string DidYouKnow { get; set; }

        public string Website { get; set; }

        public string HostName { get; set; }

        public string HostEmail { get; set; }

        public string HostPhone { get; set; }
        
        public string StudentSessionMotivation { get; set; }
    }
}
