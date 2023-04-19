using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Controllers
{
    [Route("api/timeslots")]
    [ApiController]
    public class StudentSessionTimeslotController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IStudentSessionTimeslotRepository _timeslotRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        
        public StudentSessionTimeslotController(ICompanyRepository iCompanyRepository,
            IStudentSessionTimeslotRepository iStudentSessionTimeslotRepository,
            IStudentSessionApplicationRepository iStudentSessionApplicationRepository)
        {
            _companyRepo     = iCompanyRepository;
            _timeslotRepo    = iStudentSessionTimeslotRepository;
            _applicationRepo = iStudentSessionApplicationRepository;
        }

        /// <summary>
        /// Get all timeslots by company id
        /// </summary>
        /// <param name="id">The id of the company</param>
        [HttpGet]
        [Route("company/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionTimeslot>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllTimeslots(int id)
        {
            var timeslots = await _timeslotRepo.GetAllForCompany(id);
            return Ok(timeslots);
        }

        /// <summary>
        /// Get a single timeslot
        /// </summary>
        /// <param name="id">The id of the timeslot</param>
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTimeslot(int id)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }
            return Ok(timeslot);
        }

        /// <summary>
        /// Get all companies that have timeslots
        /// </summary>
        [HttpGet]
        [Route("companies")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<PublicCompanyDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCompaniesWithTimeslot()
        {
            var allCompanies = await _companyRepo.GetAll();

            var companiesWithTimeslots = new List<Company>();
            foreach (var company in allCompanies){
                if (company.Id.HasValue){
                    var timeslots = await _timeslotRepo.GetAllForCompany(company.Id.GetValueOrDefault());
                    if (timeslots.Count() > 0)
                    {
                        companiesWithTimeslots.Add(company);
                    }
                }
            }

            var publicCompanies = companiesWithTimeslots.Select(c => new PublicCompanyDTO
            {
                Id          = c.Id.Value,
                Name        = c.Name,
                Description = c.Description,
                Website     = c.Website,
                LogoUrl     = c.LogoUrl
            });

            return Ok(publicCompanies);
        }

        /// <summary>
        /// Book a timeslot
        /// </summary>
        /// <param name="id">The id of the timeslot</param>
        [HttpPut]
        [Route("book/{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status200OK)]
        public async Task<ActionResult> BookTimeslot(int id)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }
            var companyId = timeslot.CompanyId;
            var studentId = HttpContext.User.GetStudentId().Value;
            var application = await _applicationRepo.GetByCompanyAndStudent(studentId, companyId);
            
            if(application == null)
            {
                return BadRequest();
            }

            if(application.Status != StudentSessionApplicationStatus.Accepted)
            {
                return BadRequest();
            }

            if(timeslot.StudentId != null)
            {
                return BadRequest();
            }

            if (application.Booked) 
            {
                return BadRequest();
            }

            timeslot.StudentId = studentId;
            application.Booked = true;

            await _applicationRepo.Update(application);
            await _timeslotRepo.Update(timeslot);

            return Ok(timeslot);
        }

        /// <summary>
        /// Unbook a timeslot
        /// </summary>
        /// <param name="id">The id of the timeslot</param>
        [HttpPut]
        [Route("unbook/{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status200OK)]
        public async Task<ActionResult> UnbookTimeslot(int id)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }
            var companyId = timeslot.CompanyId;
            var studentId = HttpContext.User.GetStudentId().Value;
            var application = await _applicationRepo.GetByCompanyAndStudent(studentId, companyId);

            if (application == null)
            {
                return BadRequest();
            }

            if (application.Status != StudentSessionApplicationStatus.Accepted)
            {
                return BadRequest();
            }

            if (timeslot.StudentId == null)
            {
                return BadRequest();
            }

            if (!application.Booked)
            {
                return BadRequest();
            }

            timeslot.StudentId = null;
            application.Booked = false;

            await _applicationRepo.Update(application);
            await _timeslotRepo.Update(timeslot);

            return Ok(timeslot);
        }

        /// <summary>
        /// Create a new timeslot for a company as admin
        /// </summary>
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostTimeslotAdmin(CreateStudentSessionTimeslotAdminDTO DTO)
        {
            var timeslot = new StudentSessionTimeslot
            {
                Start = DTO.Start,
                End = DTO.End,
                Location = DTO.Location,
                CompanyId = DTO.CompanyId
            };

            await _timeslotRepo.Add(timeslot);

            return Ok(timeslot);

        }

        /// <summary>
        /// Delete a timeslot
        /// </summary>
        /// <param name="id">The id of the timeslot</param>
        [HttpDelete]
        [Route("{id}")]
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
        /// Update the location for a timeslot
        /// </summary>
        /// <param name="id">The id of the timeslot</param> 
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateTimeslot(int id, UpdateStudentSessionLocationDTO DTO)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }

            if(DTO.Location != null)
            {
                timeslot.Location = DTO.Location;
            }
            
            await _timeslotRepo.Update(timeslot);

            return Ok(timeslot);
        }
    }
}
