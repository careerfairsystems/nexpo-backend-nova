
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    /// <summary>
    /// Represents a user of the system
    /// </summary>
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        [Required]
        // Unique set in DbContext
        public string Email { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        [Required]
        public Role Role { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string PhoneNr { get; set; }
        public string FoodPreferences { get; set; }
        public bool hasProfilePicture { get; set; }

        [ForeignKey(nameof(Company))]
        public int? CompanyId { get; set; }
        [JsonIgnore]
        public Company Company { get; set; }

        public bool hasCv { get; set; }

        public string profilePictureUrl { get; set; }

        public ICollection<UserNotification> UserNotifications { get; set; }
    }

    public enum Role
    {
        Administrator,
        Student,
        CompanyRepresentative,
        Volunteer
    }
}

