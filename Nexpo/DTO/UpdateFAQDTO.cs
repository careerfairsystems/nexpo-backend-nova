using Nexpo.Models;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student
    /// </summary>
    public class UpdateFAQDTO
    {
        public int Id { get; set; }

        public string Question { get; set; }

    }
}
