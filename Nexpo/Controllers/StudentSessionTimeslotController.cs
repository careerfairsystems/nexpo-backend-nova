using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Nexpo.Controllers
{
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
        [Route("timeslots/company/{id}")]
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
        [Route("timeslots")]
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
        [Route("timeslots/{id}")]
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
        /// Delete a timeslot
        /// </summary>
        [HttpDelete]
        [Route("timeslots/{id}")]
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
        [Route("timeslots/{id}")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(StudentSessionTimeslot), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutTimeslot(int id)
        {
            var timeslot = await _timeslotRepo.Get(id);
            if (timeslot == null)
            {
                return NotFound();
            }
            var companyId = timeslot.CompanyId;
            var studentId = HttpContext.User.GetStudentId().Value;

            if(!await _applicationRepo.ApplicationExists(studentId, companyId))
            {
                return BadRequest();
                
            }

            var applicationList = await _applicationRepo.GetByCompanyAndStudent(studentId, companyId);
            var application = applicationList.GetEnumerator().Current;
            if (application.booked == true || application.Status != StudentSessionApplicationStatus.Accepted)
            {
                return BadRequest();
            }

            application.booked = true;
            timeslot.booked = true;
            timeslot.StudentSessionApplication = application;

            await _applicationRepo.Update(application);
            await _timeslotRepo.Update(timeslot);

            return Ok(timeslot);
        }
    }
}
