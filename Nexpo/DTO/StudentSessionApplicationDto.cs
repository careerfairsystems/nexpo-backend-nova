using Nexpo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.DTO
{
    public class StudentSessionApplicationDto
    {
        public int? Id { get; set; }
        public string Motivation { get; set; }
        public StudentSessionApplicationStatus Status { get; set; }
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public bool Booked { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public int? StudentYear { get; set; }
        public Guild? StudentGuild { get; set; }
    }
}
