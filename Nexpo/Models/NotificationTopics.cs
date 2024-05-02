using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a notification topic
    /// </summary>
    public class NotificationTopic
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        public TopicType Topic { get; set; }

        [Required]
        [JsonIgnore]
        public User User { get; set; }



        public enum TopicType
        {
            All,
            Administratior,
            CompanyRepresentative,
            Student,

            Volunteer,
        }

    }
}

