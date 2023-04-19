

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for getting the application status (of a student session) of a student
    public class ApplicationStatusDto
    {
        public bool booked { get; set; }
        
        public bool accepted { get; set; }
    }
}