using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;

        public StudentsController(IStudentRepository iStudentRepo, IStudentSessionApplicationRepository iApplicationRepo)
        {
            _studentRepo     = iStudentRepo;
            _applicationRepo = iApplicationRepo;
        }

        /// <summary>
        /// Get a single student
        /// </summary>
        /// <param name="id">The id of the student</param>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetStudent(int id)
        {
            var student = await _studentRepo.Get(id);

            if (student == null) 
            {
                return NotFound();
            }
        
            return Ok(student);
        }
        
        /// <summary>
        /// Update a student's information
        /// </summary>
        /// <param name="id">The id of the student</param>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutStudent(int id, UpdateStudentDTO DTO)
        {
            var student = await _studentRepo.Get(id);
            if (student == null)
            {
                return NotFound();
            }

            if (DTO.Programme.HasValue && (int) DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
            {
                student.Programme = DTO.Programme.Value;
            }

            if (DTO.LinkedIn.Contains(".."))
            {
                return new BadRequestResult();
            }
            
            if (DTO.LinkedIn != null && (DTO.LinkedIn.StartsWith("https://www.linkedin.com/in/") || DTO.LinkedIn.Equals("")))
            {
                student.LinkedIn = DTO.LinkedIn;
            }
            if (DTO.MasterTitle != null)
            {
                student.MasterTitle = DTO.MasterTitle;
            }
            if (DTO.Year.HasValue && DTO.Year <= 5)
            {
                student.Year = DTO.Year.Value;
            }

            await _studentRepo.Update(student);

            return Ok(student);
        }

        /// <summary>
        /// Get the currently signed in student
        /// </summary>
        [HttpGet]
        [Route("me")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMe()
        {
            var studentId = HttpContext.User.GetStudentId();
            var student = await _studentRepo.Get(studentId.Value);
            return Ok(student);
        }

        /// <summary>
        /// Update the signed in student's information
        /// </summary>
        [HttpPut]
        [Route("me")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutMe(UpdateStudentDTO DTO)
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var student = await _studentRepo.Get(studentId);

            if (DTO.Programme.HasValue && (int) DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
            {
                student.Programme = DTO.Programme.Value;
            }
            if (DTO.LinkedIn != null && (DTO.LinkedIn.StartsWith("https://www.linkedin.com/in/") || DTO.LinkedIn.Equals("")))
            {
                student.LinkedIn = DTO.LinkedIn;
            }
            if (DTO.MasterTitle != null)
            {
                student.MasterTitle = DTO.MasterTitle;
            }
            if (DTO.Year.HasValue && DTO.Year <= 5)
            {
                student.Year = DTO.Year.Value;
            }

            await _studentRepo.Update(student);

            return Ok(student);
        }
    }
}
