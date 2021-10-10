﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        [Required]
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
        public string ProfilePictureUrl { get; set; }

        [ForeignKey(nameof(Company))]
        public int? CompanyId { get; set; }
        [JsonIgnore]
        public Company Company { get; set; }
    }

    public enum Role
    {
        Administrator,
        Student,
        CompanyRepresentative
    }
}

