using System;
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
    public class VolunteerController : ControllerBase
    {
        private readonly IVolunteerRepository _volunteerRepo;


        public VolunteerController(IVolunteerRepository iVolunteerRepo)
        {
            _volunteerRepo  = iVolunteerRepo;

        }

        /// <summary>
        /// Get a single volunteer
        /// </summary>
        /// <param name="id">The id of the volunteer</param>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(Volunteer), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetVolunteer(int id)
        {
            var volunteer = await _volunteerRepo.Get(id);

            if (volunteer == null) 
            {
                return NotFound();
            }
        
            return Ok(volunteer);
        }
        
        /// <summary>
        /// Update a volunteers's information
        /// </summary>
        /// <param name="id">The id of the volunteer</param>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Volunteer), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutVolunteer(int id, UpdateStudentDTO DTO)
        {
            var volunteer = await _volunteerRepo.Get(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            if (DTO.Programme.HasValue && (int) DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
            {
                volunteer.Programme = DTO.Programme.Value;
            }
            if (DTO.LinkedIn != null && (DTO.LinkedIn.StartsWith("https://www.linkedin.com/in/") || DTO.LinkedIn.Equals("")))
            {
                volunteer.LinkedIn = DTO.LinkedIn;
            }
            if (DTO.MasterTitle != null)
            {
                volunteer.MasterTitle = DTO.MasterTitle;
            }
            if (DTO.Year.HasValue && DTO.Year <= 5)
            {
                volunteer.Year = DTO.Year.Value;
            }

            await _volunteerRepo.Update(volunteer);

            return Ok(volunteer);
        }

        /// <summary>
        /// Get the currently signed in volunteer
        /// </summary>
        [HttpGet]
        [Route("me")]
        [Authorize(Roles = nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(Volunteer), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMe()
        {
            var volunteerId = HttpContext.User.GetVolunteerId();
            var volunteer = await _volunteerRepo.Get(volunteerId.Value);
            return Ok(volunteer);
        }

        /// <summary>
        /// Update the signed in volunteer's information
        /// </summary>
        [HttpPut]
        [Route("me")]
        [Authorize(Roles = nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(Volunteer), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutMe(UpdateStudentDTO DTO)
        {
            var volunteerId = HttpContext.User.GetVolunteerId().Value;
            var volunteer = await _volunteerRepo.Get(volunteerId);

            if (DTO.Programme.HasValue && (int) DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
            {
                volunteer.Programme = DTO.Programme.Value;
            }
            if (DTO.LinkedIn != null && (DTO.LinkedIn.StartsWith("https://www.linkedin.com/in/") || DTO.LinkedIn.Equals("")))
            {
                volunteer.LinkedIn = DTO.LinkedIn;
            }
            if (DTO.MasterTitle != null)
            {
                volunteer.MasterTitle = DTO.MasterTitle;
            }
            if (DTO.Year.HasValue && DTO.Year <= 5)
            {
                volunteer.Year = DTO.Year.Value;
            }

            await _volunteerRepo.Update(volunteer);

            return Ok(volunteer);
        }
    }
}
