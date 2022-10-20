using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;
using Nexpo.AWS;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ICompanyConnectionRepository _connectionRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly PasswordService _passwordService;

        private IAppConfiguration _appConfiguration;
        private IAws3Services _aws3Services;

        public UsersController(
            IUserRepository iUserRepo, 
            ICompanyConnectionRepository iConnectionRepo, 
            IStudentSessionApplicationRepository iApplicationRepo,
            IStudentRepository iStudentRepository,
            PasswordService passwordService,
            IAppConfiguration iAppConfiguration,
            IAws3Services iAws3Services
            )
        {
            _userRepo = iUserRepo;
            _connectionRepo = iConnectionRepo;
            _applicationRepo = iApplicationRepo;
            _studentRepo = iStudentRepository;
            _passwordService = passwordService;
            _appConfiguration = iAppConfiguration;
            _aws3Services = iAws3Services;
        }

        /// <summary>
        /// Get a list of all users
        /// </summary>
        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userRepo.GetAll());
        }

        /// <summary>
        /// Get information about a single user
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            //Only allow companies with an appliction from the student
            var userRole = HttpContext.User.GetRole();
            if (userRole == Role.CompanyRepresentative)
            {
                var companyId = HttpContext.User.GetCompanyId().Value;
                var student = await _studentRepo.FindByUser(id);
                if(student == null)
                {
                    return Forbid();
                }
                int studentId = student.Id.Value;
                if (!await _applicationRepo.ApplicationExists(studentId, companyId))
                {
                    return Forbid();
                }
            }

            var user = await _userRepo.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Update a user's information
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutUser(int id, UpdateUserDto dto)
        {
            var user = await _userRepo.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update allowed fields
            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                user.FirstName = dto.FirstName;
            }
            if (!string.IsNullOrEmpty(dto.LastName))
            {
                user.LastName = dto.LastName;
            }
            if (!string.IsNullOrEmpty(dto.PhoneNr))
            {
                user.PhoneNr = dto.PhoneNr;
            }
            if (!string.IsNullOrEmpty(dto.FoodPreferences))
            {
                user.FoodPreferences = dto.FoodPreferences;
            }
            if (!string.IsNullOrEmpty(dto.Password))
            {
                if (!_passwordService.IsStrongPassword(dto.Password))
                {
                    return BadRequest();
                }
                user.PasswordHash = _passwordService.HashPassword(dto.Password);
            }

            await _userRepo.Update(user);

            return Ok(user);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepo.Remove(user);

            return NoContent();
        }


        /// <summary>
        /// Get the signed in user
        /// </summary>
        [HttpGet]
        [Route("me")]
        [Authorize]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMe()
        {
            int userId = HttpContext.User.GetId();
            var user = await _userRepo.Get(userId);

            var response = _aws3Services.IfFileExists(user.Id.ToString());
            user.hasCv = response;

            
            return Ok(user);
        }
        
        /// <summary>
        /// Update the signed in user's information
        /// </summary>
        [HttpPut]
        [Route("me")]
        [Authorize]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutMe(UpdateUserDto dto)
        {
            var userId = HttpContext.User.GetId();
            var user = await _userRepo.Get(userId);

            // Update allowed fields
            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                user.FirstName = dto.FirstName;
            }
            if (!string.IsNullOrEmpty(dto.LastName))
            {
                user.LastName = dto.LastName;
            }
            if (!string.IsNullOrEmpty(dto.PhoneNr))
            {
                user.PhoneNr = dto.PhoneNr;
            }
            if (!string.IsNullOrEmpty(dto.FoodPreferences))
            {
                user.FoodPreferences = dto.FoodPreferences;
            }
            if (!string.IsNullOrEmpty(dto.Password))
            {
                if (!_passwordService.IsStrongPassword(dto.Password))
                {
                    return BadRequest();
                }
                user.PasswordHash = _passwordService.HashPassword(dto.Password);
            }
            await _userRepo.Update(user);

            return Ok(user);
        }

        /// <summary>
        /// Delete the signed in user
        /// </summary>
        [HttpDelete]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> DeleteMe()
        {
            var user = await _userRepo.Get(HttpContext.User.GetId());
            if (user == null)
            {
                return NotFound();
            }

            await _userRepo.Remove(user);

            return NoContent();
        }
    }
}

