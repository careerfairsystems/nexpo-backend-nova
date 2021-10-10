using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentSessionsController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IStudentSessionTimeslotRepository _timeslotRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        private readonly IStudentSessionRepository _sessionRepo;

        public StudentSessionsController(ICompanyRepository iCompanyRepository, 
            IStudentSessionTimeslotRepository iStudentSessionTimeslotRepository,
            IStudentSessionApplicationRepository iStudentSessionApplicationRepository,
            IStudentSessionRepository iStudentSessionRepository)
        {
            _companyRepo = iCompanyRepository;
            _timeslotRepo = iStudentSessionTimeslotRepository;
            _applicationRepo = iStudentSessionApplicationRepository;
            _sessionRepo = iStudentSessionRepository;
        }

        /// <summary>
        /// Get all matched student sessions as student
        /// </summary>
        [HttpGet]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(IEnumerable<StudentSession>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSessions()
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var sessions = await _sessionRepo.GetAllForStudent(studentId);
            return Ok(sessions);
        }
        
        /// <summary>
        /// Get a single student session as a student
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSession), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSession(int id)
        {
            var session = await _sessionRepo.Get(id);
            if (session == null)
            {
                return NotFound();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            if (session.StudentId != studentId)
            {
                return Forbid();
            }

            return Ok(session);
        }
        
        /// <summary>
        /// Respond to a student session as a student
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSession), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutSession(int id, UpdateSessionDto dto)
        {
            var session = await _sessionRepo.Get(id);
            if (session == null)
            {
                return NotFound();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            if (session.StudentId != studentId)
            {
                return Forbid();
            }

            session.Status = dto.Status;

            return Ok(session);
        }

        /// <summary>
        /// Get all companies available for student sessions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("companies")]
        [ProducesResponseType(typeof(IEnumerable<PublicCompanyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCompanies()
        {
            var companies = await _companyRepo.GetAllWithTimeslots();
            var publicCompanies = companies.Select(c => new PublicCompanyDto
            {
                Id = c.Id.Value,
                Name = c.Name,
                Description = c.Description,
                Website = c.Website,
                LogoUrl = c.LogoUrl
            });

            return Ok(publicCompanies);
        }

        /// <summary>
        /// Create a new application for a student session
        /// </summary>
        [HttpPost]
        [Route("applications")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionApplicationDto), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostApplication(CreateStudentSessionApplicationDto dto)
        {
            // Check that the company accepts applications
            var company = await _companyRepo.GetWithChildren(dto.CompanyId);
            if (company.StudentSessionTimeslots.Count() == 0)
            {
                return BadRequest();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            var application = new StudentSessionApplication
            {
                Motivation = dto.Motivation,
                CompanyId = dto.CompanyId,
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
        /// Get all applications made by the signed in student
        /// </summary>
        [HttpGet]
        [Route("applications/my/student")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionApplicationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsForStudent()
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var applications = await _applicationRepo.GetAllForStudent(studentId);
            var studentApplications = applications.Select(a => new StudentSessionApplicationDto
            {
                Id = a.Id.Value,
                Motivation = a.Motivation,
                CompanyId = a.CompanyId
            });

            return Ok(studentApplications);
        }

        /// <summary>
        /// Get information about an application as a student
        /// </summary>
        [HttpGet]
        [Route("applications/{id}/student")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionApplicationDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationStudent(int id)
        {
            var application = await _applicationRepo.Get(id);

            if (application == null)
            {
                return NotFound();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            if (application.StudentId != studentId)
            {
                return Forbid();
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
        /// Modify a student session application
        /// </summary>
        [HttpPut]
        [Route("applications/{id}/student")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionApplicationDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutApplicationStudent(int id, UpdateStudentSessionApplicationStudentDto dto)
        {
            var application = await _applicationRepo.Get(id);

            if (application == null)
            {
                return NotFound();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            if (application.StudentId != studentId)
            {
                return Forbid();
            }

            // Update allowed fields
            application.Motivation = dto.Motivation;

            await _applicationRepo.Update(application);

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
        [Route("applications/my/company")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionApplication>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsForCompany()
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var applications = await _applicationRepo.GetAllForCompany(companyId);

            return Ok(applications);
        }
        /// <summary>
        /// Get information about an application as a company
        /// </summary>
        [HttpGet]
        [Route("applications/{id}/company")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(StudentSessionApplication), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationCompany(int id)
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

            return Ok(application);
        }

        /// <summary>
        /// Modify a student's application (rate it)
        /// </summary>
        [HttpPut]
        [Route("applications/{id}/company")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(StudentSessionApplication), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutApplicationCompany(int id, UpdateStudentSessionApplicationCompanyDto dto)
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

            // Update allowed fields
            application.Rating = dto.Rating;

            await _applicationRepo.Update(application);

            return Ok(application);
        }

        /// <summary>
        /// Delete a student session application
        /// </summary>
        [HttpDelete]
        [Route("applications/{id}")]
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

        /// <summary>
        /// Get available timeslots for the signed in company
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("timeslots")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionTimeslot>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTimeslots()
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var timeslots = await _timeslotRepo.GetAllForCompany(companyId);
            return Ok(timeslots);
        }

        /// <summary>
        /// Create a new timeslot for a company
        /// </summary>
        [HttpPost]
        [Route("timeslots")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostTimeslot(CreateStudentSessionTimeslotDto dto)
        {
            var timeslot = new StudentSessionTimeslot
            {
                Start = dto.Start,
                End = dto.End,
                Location = dto.Location
            };

            await _timeslotRepo.Add(timeslot);

            return CreatedAtAction(nameof(GetTimeslot), new { id = timeslot.Id }, timeslot);

        }

        /// <summary>
        /// Get a single timeslot
        /// </summary>
        [HttpGet]
        [Route("timeslots/{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTimeslot(int id)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }

            var userRole = HttpContext.User.GetRole();
            if (userRole == Role.CompanyRepresentative)
            {
                var companyId = HttpContext.User.GetCompanyId().Value;
                if (timeslot.CompanyId != companyId)
                {
                    return Forbid();
                }
            }

            return Ok(timeslot);
        }

        /// <summary>
        /// Modify a timeslot
        /// </summary>
        [HttpPut]
        [Route("timeslots/{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutTimeslot(int id, StudentSessionTimeslot dto)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }

            // Update fields
            timeslot.Start = dto.Start;
            timeslot.End = dto.End;
            timeslot.Location = dto.Location;

            await _timeslotRepo.Update(timeslot);

            return Ok(timeslot);
        }
        
        /// <summary>
        /// Delete a timeslot
        /// </summary>
        [HttpDelete]
        [Route("timeslots/{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteTimeslot(int id)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }

            await _timeslotRepo.Remove(timeslot);

            return NoContent();
        }
        
        /// <summary>
        /// Match applications and timeslots
        /// </summary>
        [HttpGet]
        [Route("match")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> MatchApplications()
        {
            // Only get unmatched timeslots and applications
            var timeslots = await _timeslotRepo.GetAllUnassigned();
            var applications = await _applicationRepo.GetAllUnassigned();

            // Group the timeslots and applications by company and deal with them one company at a time
            var timeslotsByCompany = timeslots.GroupBy(t => t.CompanyId);
            var applicationsByCompany = applications.OrderByDescending(a => a.Rating).GroupBy(a => a.CompanyId);
            foreach (var companyTimeslots in timeslotsByCompany)
            {
                // Get applications for the available timeslots, if any
                var companyApplications = applicationsByCompany.Where(a => a.Key == companyTimeslots.Key).FirstOrDefault();
                if (companyApplications == null)
                {
                    continue;
                }

                // Create the matches by zipping the two lists and creating studentsessions, the zipping takes care of size mismatches
                var matches = companyTimeslots.Zip(companyApplications);
                foreach (var (timeslot, application) in matches)
                {
                    var session = new StudentSession
                    {
                        StudentId = application.StudentId,
                        StudentSessionTimeslotId = timeslot.Id.Value,
                        StudentSessionApplicationId = application.Id.Value
                    };
                    // Persist in db
                    await _sessionRepo.Add(session);

                    // Update links to session
                    timeslot.StudentSessionId = session.Id.Value;
                    await _timeslotRepo.Update(timeslot);

                    // Update links to session
                    application.StudentSessionId = session.Id.Value;
                    await _applicationRepo.Update(application);
                }
            }

            return NoContent();
        }
    }
}

