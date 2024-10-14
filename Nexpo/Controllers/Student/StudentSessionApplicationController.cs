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
using Nexpo.Services;

namespace Nexpo.Controllers
{
    [Route("api/applications")]
    [ApiController]
    public class StudentSessionsApplicationController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IStudentRepository _studentRepository;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IStudentSessionTimeslotRepository _timeslotRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;


        public StudentSessionsApplicationController(
            ICompanyRepository iCompanyRepository,
            IStudentSessionTimeslotRepository iStudentSessionTimeslotRepository,
            IStudentSessionApplicationRepository iStudentSessionApplicationRepository,
            IStudentRepository iStudentRepository,
            IVolunteerRepository iVolunteerRepository,
            IUserRepository iUserRepository,
            IEmailService iEmailService)
        {
            _companyRepo = iCompanyRepository;
            _timeslotRepo = iStudentSessionTimeslotRepository;
            _applicationRepo = iStudentSessionApplicationRepository;
            _studentRepository = iStudentRepository;
            _volunteerRepository = iVolunteerRepository;
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
        public async Task<ActionResult> RespondToApplication(int id, UpdateSessionDTO DTO)
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
            application.Status = DTO.Status;
            await _applicationRepo.Update(application);

            if (application.Status != oldStatus)
            {
                var company = await _companyRepo.Get(companyId);
                var student = await _studentRepository.Get(application.StudentId);
                var volunteer = await _volunteerRepository.Get(application.StudentId);

                int userId = student?.UserId ?? volunteer?.UserId ?? -1;

                var user = await _userRepository.Get(userId);

                switch (application.Status)
                {
                    case StudentSessionApplicationStatus.Accepted:
                        await _emailService.SendApplicationAcceptedEmail(company, user);
                        break;
                    case StudentSessionApplicationStatus.Declined:
                        await _emailService.SendApplicationRejectedEmail(company, user);
                        break;
                    case StudentSessionApplicationStatus.Pending:
                        await _emailService.SendApplicationPendingEmail(company, user);
                        break;
                    default:
                        break;
                }
            }

            return Ok(application);
        }

        /// <summary>
        /// Create a new application for a student session
        /// </summary>
        [HttpPost]
        [Route("company/{id}")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer) + "," + nameof(Role.Admin))]
        [ProducesResponseType(typeof(StudentSessionApplication), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostApplication(int id, UpdateStudentSessionApplicationStudentDTO DTO)
        {
            var motivation = string.IsNullOrEmpty(DTO?.Motivation) ? "**NO MOTIVATION ADDED**" : DTO.Motivation;

            // Check that the company accepts applications
            var company = await _companyRepo.GetWithChildren(id);
            if (company.StudentSessionTimeslots.Count() == 0)
            {
                return Conflict();
            }

            var userRole = HttpContext.User.GetRole();
            var applierId = -1;

            if (userRole == Role.Student)
            {
                applierId = HttpContext.User.GetStudentId().Value;
            }

            if (userRole == Role.Volunteer)
            {
                applierId = HttpContext.User.GetVolunteerId().Value;
            }

            if (await _applicationRepo.ApplicationExists(applierId, id))
            {
                var current = await _applicationRepo.GetByCompanyAndStudent(applierId, id);
                current.Motivation = motivation;
                await _applicationRepo.Update(current);
                return CreatedAtAction(nameof(GetApplicationStudent), new { id = current.Id }, current);
            }
            else
            {
                var application = new StudentSessionApplication
                {
                    Motivation = motivation,
                    CompanyId = id,
                    StudentId = applierId
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
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.CompanyRepresentative) + "," + nameof(Role.Volunteer))]
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

            if (userRole == Role.Volunteer)
            {
                var volunteerId = HttpContext.User.GetVolunteerId().Value;
                if (application.StudentId != volunteerId)
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
        [ProducesResponseType(typeof(IEnumerable<StudentSessionApplicationDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsForCompany()
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var applications = await _applicationRepo.GetAllForCompany(companyId);
            var studentApplications = new List<StudentSessionApplicationDTO>();

            foreach (var application in applications)
            {
                var student = await _studentRepository.Get(application.StudentId);
                var volunteer = await _volunteerRepository.Get(application.StudentId);

                int userId = student?.UserId ?? volunteer?.UserId ?? -1;
                var user = await _userRepository.Get(userId);

                int? studentYear = student?.Year ?? volunteer?.Year;
                Programme? studentProgramme = student?.Programme ?? volunteer?.Programme;


                var DTO = new StudentSessionApplicationDTO
                {
                    Id = application.Id,
                    Motivation = application.Motivation,
                    Status = application.Status,
                    StudentId = application.StudentId,
                    CompanyId = application.CompanyId,
                    Booked = application.Booked,
                    StudentFirstName = user.FirstName,
                    StudentLastName = user.LastName,
                    StudentYear = studentYear,
                    StudentProgramme = studentProgramme
                };

                studentApplications.Add(DTO);
            }
            return Ok(studentApplications);
        }

        /// <summary>
        /// Get all applications made by the signed in Student or Volunteer
        /// </summary>
        [HttpGet]
        [Route("my/student")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionApplication>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsFromStudent()
        {
            var userRole = HttpContext.User.GetRole();
            var applierId = -1;

            if (userRole == Role.Student)
            {
                applierId = HttpContext.User.GetStudentId().Value;
            }

            if (userRole == Role.Volunteer)
            {
                applierId = HttpContext.User.GetVolunteerId().Value;
            }

            var applications = await _applicationRepo.GetAllForApplier(applierId);

            return Ok(applications);
        }

        /// <summary>
        /// Checks if application exists for given companyId and is accepted and is booked
        /// </summary>
        /// <param name="id">Company Id</param>
        [HttpGet]
        [Route("accepted/{id}")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(ApplicationStatusDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult> applicationAccepted(int id)
        {
            var userRole = HttpContext.User.GetRole();
            var applierId = -1;

            if (userRole == Role.Student)
            {
                applierId = HttpContext.User.GetStudentId().Value;
            }

            if (userRole == Role.Volunteer)
            {
                applierId = HttpContext.User.GetVolunteerId().Value;
            }

            var applicationExists = await _applicationRepo.ApplicationExists(applierId, id);

            if (!applicationExists)
            {
                return BadRequest();
            }

            var application = await _applicationRepo.GetByCompanyAndStudent(applierId, id);
            if (application.Status != StudentSessionApplicationStatus.Accepted)
            {
                return Ok(new ApplicationStatusDTO
                {
                    accepted = false,
                    booked = application.Booked
                });
            }
            return Ok(new ApplicationStatusDTO
            {
                accepted = true,
                booked = application.Booked
            });
        }

        /// <summary>
        /// Delete a student session application
        /// </summary>
        /// <param name="id">Application Id</param>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.CompanyRepresentative) + "," + nameof(Role.Volunteer))]
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

            if (userRole == Role.Volunteer)
            {
                var volunteer = HttpContext.User.GetStudentId().Value;
                if (application.StudentId != volunteer)
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

