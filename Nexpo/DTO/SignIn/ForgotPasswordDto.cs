﻿
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for requesting a password reset email to be sent to the user
    /// </summary>
    public class ForgotPasswordDto
    {
        [Required]
        public string Email { get; set; }
    }
}
