using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using System;
using System.Collections.Generic;
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
        [Route("{id}")]
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

            var test = await _applicationRepo.GetByCompanyAndStudent(studentId,companyId);
            Console.WriteLine(test);
            var application = await _applicationRepo.GetByCompanyAndStudent(studentId, companyId);
            
            if(application == null)
            {
                return BadRequest();
            }

            if(application.Status != StudentSessionApplicationStatus.Accepted)
            {
                return BadRequest();
            }
            if (application.booked == true)
            {
                return BadRequest();
            }

            application.booked = true;
            timeslot.booked = true;

            await _applicationRepo.Update(application);
            await _timeslotRepo.Update(timeslot);

            return Ok(timeslot);
        }
    }
}
