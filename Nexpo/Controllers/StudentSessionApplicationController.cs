using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;

namespace Nexpo.Controllers
{
    [Route("api/applications")]
    [ApiController]
    public class StudentSessionsApplicationController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentSessionTimeslotRepository _timeslotRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public StudentSessionsApplicationController(ICompanyRepository iCompanyRepository,
            IStudentSessionTimeslotRepository iStudentSessionTimeslotRepository,
            IStudentSessionApplicationRepository iStudentSessionApplicationRepository,
            IStudentRepository iStudentRepository,
            IUserRepository iUserRepository,
            EmailService iEmailService)
        {
            _companyRepo = iCompanyRepository;
            _timeslotRepo = iStudentSessionTimeslotRepository;
            _applicationRepo = iStudentSessionApplicationRepository;
            _studentRepository = iStudentRepository;
            _userRepository = iUserRepository;
            _emailService = iEmailService;
        }


        /// <summary>
        /// Respond to a student session as a company representative
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(StudentSessionApplication), StatusCodes.Status200OK)]
        public async Task<ActionResult> RespondToApplication(int id, UpdateSessionDto dto)
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
            var oldStatus = application.Status;
            application.Status = dto.Status;
            await _applicationRepo.Update(application);
            if(application.Status != oldStatus && application.Status == StudentSessionApplicationStatus.Accepted)
            {
                var company = await _companyRepo.Get(companyId);
                var student = await _studentRepository.Get(application.StudentId);
                var user = await _userRepository.Get(student.UserId);

                await _emailService.SendApplicationAcceptedEmail(company, user);
            }
            return Ok(application);
        }

        /// <summary>
        /// Create a new application for a student session
        /// </summary>
        [HttpPost]
        [Route("company/{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionApplication), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostApplication(int id, UpdateStudentSessionApplicationStudentDto dto)
        {
            // Check that the company accepts applications
            var company = await _companyRepo.GetWithChildren(id);
            if (company.StudentSessionTimeslots.Count() == 0)
            {
                return Conflict();
            }

            var studentId = HttpContext.User.GetStudentId().Value;

            if (await _applicationRepo.ApplicationExists(studentId, id))
            {
                var current = await _applicationRepo.GetByCompanyAndStudent(studentId, id);
                current.Motivation = dto.Motivation;
                await _applicationRepo.Update(current);
                return CreatedAtAction(nameof(GetApplicationStudent), new { id = current.Id }, current);
            }
            else
            {
                var application = new StudentSessionApplication
                {
                    Motivation = dto.Motivation,
                    CompanyId = id,
                    StudentId = studentId
                };
                await _applicationRepo.Add(application);
                return CreatedAtAction(nameof(GetApplicationStudent), new { id = application.Id }, application);
            }
        }

        /// <summary>
        /// Get information about an application as company or student
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(StudentSessionApplication), StatusCodes.Status200OK)]
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

          
            return Ok(application);
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
        /// Get all applications made by the signed in Student
        /// </summary>
        [HttpGet]
        [Route("my/student")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionApplication>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsFromStudent()
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var applications = await _applicationRepo.GetAllForStudent(studentId);

            return Ok(applications);
        }

        /// <summary>
        /// Checks if application exists and is accepted and is booked
        /// </summary>
        [HttpGet]
        [Route("accepted/{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(ApplicationStatusDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> applicationAccepted(int id)
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var exists = await _applicationRepo.ApplicationExists(studentId, id);
            if (!exists)
            {
                return BadRequest();
            }
            var application = await _applicationRepo.GetByCompanyAndStudent(studentId, id);
            
            if(application.Status != StudentSessionApplicationStatus.Accepted)
            {
                return Ok(new ApplicationStatusDto
                {
                    accepted = false,
                    booked = application.Booked
                });
            }
            return Ok(new ApplicationStatusDto
            {
                accepted=true,
                booked=application.Booked
            });
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

