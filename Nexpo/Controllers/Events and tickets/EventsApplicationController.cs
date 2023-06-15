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
    public class EventsApplicationController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEventApplicationRepository _applicationRepo;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ITicketRepository _ticketRepository;


        public EventsApplicationController(
            IEventRepository iEventRepository,
            IEventApplicationRepository iEventApplicationRepository,
            IStudentRepository iStudentRepository,
            IUserRepository iUserRepository,
            IEmailService iEmailService,
            ICompanyRepository iCompanyRepository,
            ITicketRepository iTicketRepository
            )
        {
            _eventRepository   = iEventRepository;
            _companyRepository = iCompanyRepository;
            _applicationRepo   = iEventApplicationRepository;
            _studentRepository = iStudentRepository;
            _userRepository    = iUserRepository;
            _ticketRepository  = iTicketRepository;
            _emailService      = iEmailService;

        }

        /// <summary>
        /// Respond to a student event application as a company representative
        /// </summary>
        /// <param name="id">The id of the application</param>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(EventApplication), StatusCodes.Status200OK)]
        public async Task<ActionResult> RespondToApplication(int id, UpdateEventApplicationStatusDTO DTO)
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

            if(application.Status != oldStatus && application.Status == EventApplicationStatus.Accepted)
            {
                var company = await _companyRepository.Get(companyId);
                var student = await _studentRepository.Get(application.StudentId);
                var user = await _userRepository.Get(student.UserId);
                var _event = await _eventRepository.Get(application.EventId);

                var ticket = new Ticket
                {
                    PhotoOk = application.PhotoOk,
                    EventId = application.EventId,
                    UserId  = student.UserId
                };

                await _ticketRepository.Add(ticket);

                await _emailService.SendEventApplicationAcceptedEmail(company, _event, user);
            }

            return Ok(application);
        }

        /// <summary>
        /// Create a new application for a event as a student
        /// </summary>
        [HttpPost]
        [Route("apply")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(EventApplication), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostApplication(UpdateEventApplicationStudentDTO DTO)
        {
            var _event = await _eventRepository.Get(DTO.EventId);
            if(_event == null)
            {
                return NotFound();
            }

            var studentId = HttpContext.User.GetStudentId().Value;

            if (await _applicationRepo.ApplicationExists(studentId, DTO.EventId))
            {
                var current = await _applicationRepo.GetByEventAndStudent(studentId, DTO.EventId);
                current.Motivation = DTO.Motivation;
                await _applicationRepo.Update(current);
                return CreatedAtAction(nameof(GetApplicationStudent), new { id = current.Id }, current);
            }
            else
            {
                var application = new EventApplication
                {
                    Motivation = DTO.Motivation,
                    StudentId = studentId,
                    EventId = DTO.EventId,
                    CompanyId = DTO.CompanyId,
                    PhotoOk = DTO.PhotoOk,
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
        [ProducesResponseType(typeof(EventApplication), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(IEnumerable<EventApplicationDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsForCompany()
        {
            var eventId = HttpContext.User.GetCompanyId().Value;
            var applications = await _applicationRepo.GetAllForCompany(eventId);
            var studentApplications = new List<EventApplicationDTO>();

            foreach (var application in applications){
                var student = await _studentRepository.Get(application.StudentId);
                var user = await _userRepository.Get(student.UserId);

                var DTO = new EventApplicationDTO
                {
                    Id               = application.Id,
                    Motivation       = application.Motivation,
                    Status           = application.Status,
                    StudentId        = application.StudentId,
                    EventId          = application.EventId,
                    Booked           = application.Booked,
                    StudentFirstName = user.FirstName,
                    StudentLastName  = user.LastName,
                    StudentYear      = student.Year,
                    StudentProgramme = student.Programme
                };

                studentApplications.Add(DTO);
            }
            return Ok(studentApplications);
        }

        /// <summary>
        /// Get all applications made by the signed in Student
        /// </summary>
        [HttpGet]
        [Route("my/student")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(IEnumerable<EventApplication>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetApplicationsFromStudent()
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var applications = await _applicationRepo.GetAllForStudent(studentId);

            return Ok(applications);
        }

        /// <summary>
        /// Checks if application exists for given eventId and is accepted and is booked
        /// Lite konstig IMO. Kanske inte ha med? Eller gör om? Ta även in studentId
        /// </summary>
        /// <param name="id">Event Id</param>
        [HttpGet]
        [Route("accepted/{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(ApplicationStatusDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult> applicationAccepted(int id)
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var applicationExists = await _applicationRepo.ApplicationExists(studentId, id);

            if (!applicationExists)
            {
                return BadRequest();
            }

            var application = await _applicationRepo.GetByEventAndStudent(studentId, id);
            if(application.Status != EventApplicationStatus.Accepted)
            {
                return Ok(new ApplicationStatusDTO
                {
                    accepted = false,
                    booked = application.Booked
                });
            }
            return Ok(new ApplicationStatusDTO
            {
                accepted=true,
                booked=application.Booked
            });
        }

        /// <summary>
        /// Delete a student event application
        /// </summary>
        /// <param name="id">Application Id</param>
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
                var eventId = HttpContext.User.GetCompanyId().Value;
                if (application.EventId != eventId)
                {
                    return Forbid();
                }
            }

            await _applicationRepo.Remove(application);
            return NoContent();
        }
    }
}

