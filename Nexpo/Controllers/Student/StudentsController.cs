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
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        private readonly IVolunteerRepository _volunteerRepo;

        public StudentsController(IStudentRepository studentRepo, IStudentSessionApplicationRepository applicationRepo, IVolunteerRepository volunteerRepo)
        {
            _studentRepo = studentRepo;
            _applicationRepo = applicationRepo;
            _volunteerRepo = volunteerRepo;
        }

        /// <summary>
        /// Get a single student
        /// </summary>
        /// <param name="id">The id of the student</param>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetStudent(int id)
        {
            var student = await _studentRepo.Get(id);
            var volunteer = await _volunteerRepo.Get(id);

            if (volunteer != null)
            {
                return Ok(volunteer);

            }

            if (student != null)
            {
                return Ok(student);

            }

            return NotFound();


        }

        /// <summary>
        /// Update a student's information
        /// 
        /// Unfortunly very badly implement due to time preassure :(
        /// </summary>
        /// <param name="id">The id of the student</param>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutStudent(int id, UpdateStudentDTO DTO)
        {
            var student = await _studentRepo.Get(id);
            var volunteer = await _volunteerRepo.Get(id);

            if (volunteer != null)
            {
                if (DTO.Programme.HasValue && (int)DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
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

            if (student == null)
            {
                return NotFound();
            }

            if (DTO.Programme.HasValue && (int)DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
            {
                student.Programme = DTO.Programme.Value;
            }
            if (DTO.LinkedIn != null && (DTO.LinkedIn.StartsWith("https://www.linkedin.com/in/") || DTO.LinkedIn.Equals("")))
            {
                student.LinkedIn = DTO.LinkedIn;
            }
            if (DTO.MasterTitle != null)
            {
                student.MasterTitle = DTO.MasterTitle;
            }
            if (DTO.Year.HasValue && DTO.Year <= 5)
            {
                student.Year = DTO.Year.Value;
            }

            await _studentRepo.Update(student);

            return Ok(student);
        }

        /// <summary>
        /// Get the currently signed in student
        /// </summary>
        [HttpGet]
        [Route("me")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMe()
        {
            var studentId = HttpContext.User.GetStudentId();
            var student = await _studentRepo.Get(studentId.Value);

            if (student != null)
            {
                return Ok(student);
            }

            var volunteerId = HttpContext.User.GetVolunteerId();
            var volunteer = await _volunteerRepo.Get(volunteerId.Value);

            return Ok(volunteer);
        }

        /// <summary>
        /// Update the signed in student's information
        /// 
        /// Unfortunly very badly implement due to time preassure :(
        /// </summary>
        [HttpPut]
        [Route("me")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutMe(UpdateStudentDTO DTO)
        {
            var studentId = HttpContext.User.GetStudentId().GetValueOrDefault(0);
            var student = await _studentRepo.Get(studentId);

            var volunteerId = HttpContext.User.GetVolunteerId().GetValueOrDefault(0);
            var volunteer = await _volunteerRepo.Get(volunteerId);


            if (volunteer != null)
            {
                if (DTO.Programme.HasValue && (int)DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
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

            if (DTO.Programme.HasValue && (int)DTO.Programme.Value < Enum.GetNames(typeof(Programme)).Length)
            {
                student.Programme = DTO.Programme.Value;
            }
            if (DTO.LinkedIn != null && (DTO.LinkedIn.StartsWith("https://www.linkedin.com/in/") || DTO.LinkedIn.Equals("")))
            {
                student.LinkedIn = DTO.LinkedIn;
            }
            if (DTO.MasterTitle != null)
            {
                student.MasterTitle = DTO.MasterTitle;
            }
            if (DTO.Year.HasValue && DTO.Year <= 5)
            {
                student.Year = DTO.Year.Value;
            }

            await _studentRepo.Update(student);

            return Ok(student);
        }
    }
}

