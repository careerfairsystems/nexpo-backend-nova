using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;
using Nexpo.AWS;

namespace Nexpo.Controllers
{
    /// <summary>
    /// Controller for managing more complex role functionality
    /// Currently only used for updating roles 
    ///        - but could be expanded to include more complex role functionality
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IVolunteerRepository _volunteerRepo;


        public RoleController(
            IUserRepository iUserRepo,
            IStudentSessionApplicationRepository iApplicationRepo,
            IStudentRepository iStudentRepository,
            IVolunteerRepository iVolunteerRepository
        )
        {
            _userRepo = iUserRepo;
            _applicationRepo = iApplicationRepo;
            _studentRepo = iStudentRepository;
            _volunteerRepo = iVolunteerRepository;
        }


        /// <summary>
        /// Get information about a single user
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetRole(int id)
        {
            var user = await _userRepo.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            var role = User.GetRole();

            return Ok(role);
        }

        /// <summary>
        /// Update a user's information
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutRole(int id, UpdateUserDTO DTO)
        {
            var user = await _userRepo.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!DTO.Role.HasValue)
            {
                return BadRequest();
            }

            if(user.Role == (Role)DTO.Role){
                return BadRequest();
            }

            if((Role)DTO.Role == Role.CompanyRepresentative){
                return BadRequest();
            }

            // Delete the user from its former repo

            var student = _studentRepo.FindByUser((int)user.Id);

            if (student != null)
                await _studentRepo.Remove(student.Result);

            var volunteer = _volunteerRepo.FindByUser((int)user.Id);

            if (volunteer != null)
                await _volunteerRepo.Remove(volunteer.Result);


            // Put the user in their new repo

            user.Role = (Role)DTO.Role;

            if (user.Role == Role.Student)
            {
                var newStudent = new Student
                {
                    UserId = user.Id.Value
                };
                await _studentRepo.Add(newStudent);
            }
            
            if (user.Role == Role.Volunteer)
            {
                var newVolunteer = new Volunteer
                {
                    UserId = user.Id.Value
                };
                await _volunteerRepo.Add(newVolunteer);
            }


            await _userRepo.Update(user);

            return Ok(user);
        }

    }

}

