using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
            _companyRepo = iCompanyRepository;
            _timeslotRepo = iStudentSessionTimeslotRepository;
            _applicationRepo = iStudentSessionApplicationRepository;
        }

        /// <summary>
        /// Get all timeslots by company id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("company/{id}")]
        [ProducesResponseType(typeof(IEnumerable<StudentSessionTimeslot>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllTimeslots(int id)
        {
            var timeslots = await _timeslotRepo.GetAllForCompany(id);
            return Ok(timeslots);
        }

        /// <summary>
        /// Create a new timeslot for a company
        /// </summary>
        [HttpPost]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
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
        [Route("{id}")]
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
        [ProducesResponseType(typeof(IEnumerable<PublicCompanyDto>), StatusCodes.Status200OK)]
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

            var publicCompanies = companiesWithTimeslots.Select(c => new PublicCompanyDto
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
        /// Delete a timeslot
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteTimeslot(int id)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }

            var companyId = HttpContext.User.GetCompanyId().Value;
            if (timeslot.CompanyId != companyId)
            {
                return Forbid();
            }

            await _timeslotRepo.Remove(timeslot);

            return NoContent();
        }

        /// <summary>
        /// Book a timeslot
        /// </summary>
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
        //[Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostTimeslotAdmin(CreateStudentSessionTimeslotAdminDto dto)
        {
            var timeslot = new StudentSessionTimeslot
            {
                Start = dto.Start,
                End = dto.End,
                Location = dto.Location,
                CompanyId = dto.CompanyId
            };

            await _timeslotRepo.Add(timeslot);

            return Ok(timeslot);

        }
    }
}
