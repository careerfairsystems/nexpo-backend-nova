﻿
using System.ComponentModel.DataAnnotations;

namespace Nexpo.DTO
{
    /// <summary>
    /// DTO for updating a student session application
    /// </summary>
    public class UpdateStudentSessionApplicationStudentDTO
    {
        [Required]
        public string Motivation { get; set; }
    }
}
