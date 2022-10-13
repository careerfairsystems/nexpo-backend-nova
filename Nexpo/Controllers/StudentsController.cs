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
        private readonly ICompanyConnectionRepository _connectionRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;

        public StudentsController(IStudentRepository iStudentRepo, ICompanyConnectionRepository iConnectionRepo, IStudentSessionApplicationRepository iApplicationRepo)
        {
            _studentRepo = iStudentRepo;
            _connectionRepo = iConnectionRepo;
            _applicationRepo = iApplicationRepo;
        }

        /// <summary>
        /// Get a single student
        /// </summary>
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
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutStudent(int id, UpdateStudentDto dto)
        {
            var student = await _studentRepo.Get(id);
            if (student == null)
            {
                return NotFound();
            }

            if (dto.Guild.HasValue)
            {
                student.Guild = dto.Guild.Value;
            }
            if (!string.IsNullOrEmpty(dto.LinkedIn))
            {
                student.LinkedIn = dto.LinkedIn;
            }
            if (!string.IsNullOrEmpty(dto.MasterTitle))
            {
                student.MasterTitle = dto.MasterTitle;
            }
            if (dto.Year.HasValue)
            {
                student.Year = dto.Year.Value;
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
        public async Task<ActionResult> PutMe(UpdateStudentDto dto)
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var student = await _studentRepo.Get(studentId);

            if (dto.Guild.HasValue)
            {
                student.Guild = dto.Guild.Value;
            }
            if (!string.IsNullOrEmpty(dto.LinkedIn))
            {
                student.LinkedIn = dto.LinkedIn;
            }
            if (!string.IsNullOrEmpty(dto.MasterTitle))
            {
                student.MasterTitle = dto.MasterTitle;
            }
            if (dto.Year.HasValue)
            {
                student.Year = dto.Year.Value;
            }

            await _studentRepo.Update(student);

            return Ok(student);
        }
    }
}

