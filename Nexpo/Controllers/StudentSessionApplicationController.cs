using System.Collections.Generic;
using System.Linq;
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
    [Route("api/applications")]
    [ApiController]
    public class StudentSessionsApplicationController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IStudentSessionTimeslotRepository _timeslotRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;

        public StudentSessionsApplicationController(ICompanyRepository iCompanyRepository,
            IStudentSessionTimeslotRepository iStudentSessionTimeslotRepository,
            IStudentSessionApplicationRepository iStudentSessionApplicationRepository)
        {
            _companyRepo = iCompanyRepository;
            _timeslotRepo = iStudentSessionTimeslotRepository;
            _applicationRepo = iStudentSessionApplicationRepository;
        }


        /// <summary>
        /// Respond to a student session as a company representative
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(StudentSessionApplication), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutSession(int id, UpdateSessionDto dto)
        {
            var application = await _applicationRepo.Get(id);
            if (application == null)
            {
                return NotFound();
            }

            var companyId = HttpContext.User.GetCompanyId().Value;
            if (application.CompanyId != companyId)
            {
                return Forbid();
            }

            application.Status = dto.Status;

            return Ok(application);
        }

        /// <summary>
        /// Create a new application for a student session
        /// </summary>
        [HttpPost]
        [Route("company/{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionApplicationDto), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostApplication(int id, string motivation)
        {
            // Check that the company accepts applications
            var company = await _companyRepo.GetWithChildren(id);
            if (company.StudentSessionTimeslots.Count() == 0)
            {
                return BadRequest();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            var application = new StudentSessionApplication
            {
                Motivation = motivation,
                CompanyId = id,
                StudentId = studentId
            };

            await _applicationRepo.Add(application);

            var studentApplication = new StudentSessionApplicationDto
            {
                Id = application.Id.Value,
                Motivation = application.Motivation,
                CompanyId = application.CompanyId
            };

            return CreatedAtAction(nameof(GetApplicationStudent), new { id = studentApplication.Id }, studentApplication);
        }

        /// <summary>
        /// Get information about an application as company or student
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(StudentSessionApplicationDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationStudent(int id)
        {
            var application = await _applicationRepo.Get(id);
            if (application == null)
            {
                return NotFound();
            }

            var userRole = HttpContext.User.GetRole();
            if (userRole == Role.Student)
            {
                var studentId = HttpContext.User.GetStudentId().Value;
                if (application.StudentId != studentId)
                {
                    return Forbid();
                }
            }
            if (userRole == Role.CompanyRepresentative)
            {
                var companyId = HttpContext.User.GetCompanyId().Value;
                if (application.CompanyId != companyId)
                {
                    return Forbid();
                }
            }

            var studentApplication = new StudentSessionApplicationDto
            {
                Id = application.Id.Value,
                Motivation = application.Motivation,
                CompanyId = application.CompanyId
            };
            return Ok(studentApplication);
        }

        /// <summary>
        /// Get all applications made to the signed in company
        /// </summary>
        [HttpGet]
        [Route("my/company")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionApplication>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsForCompany()
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var applications = await _applicationRepo.GetAllForCompany(companyId);

            return Ok(applications);
        }

        /// <summary>
        /// Delete a student session application
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteApplication(int id)
        {
            var application = await _applicationRepo.Get(id);
            if (application == null)
            {
                return NotFound();
            }

            var userRole = HttpContext.User.GetRole();
            if (userRole == Role.Student)
            {
                var studentId = HttpContext.User.GetStudentId().Value;
                if (application.StudentId != studentId)
                {
                    return Forbid();
                }
            }
            if (userRole == Role.CompanyRepresentative)
            {
                var companyId = HttpContext.User.GetCompanyId().Value;
                if (application.CompanyId != companyId)
                {
                    return Forbid();
                }
            }

            await _applicationRepo.Remove(application);
            return NoContent();
        }
    }
}

