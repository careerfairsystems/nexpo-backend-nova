using System.Collections.Generic;
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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IStudentSessionApplicationRepository _applicationRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly PasswordService _passwordService;

        private IS3Configuration _appConfiguration;
        private IAws3Services _aws3Services;
        private IS3Configuration _s3Configuration;

        public UsersController(
            IUserRepository iUserRepo,
            IStudentSessionApplicationRepository iApplicationRepo,
            IStudentRepository iStudentRepository,
            PasswordService passwordService,
            IS3Configuration iAppConfiguration,
            IAws3Services iAws3Services,
            IS3Configuration s3Configuration
            )
        {
            _userRepo = iUserRepo;
            _applicationRepo = iApplicationRepo;
            _studentRepo = iStudentRepository;
            _passwordService = passwordService;
            _appConfiguration = iAppConfiguration;
            _s3Configuration = s3Configuration;
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
                if (student == null)
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
        public async Task<IActionResult> PutUser(int id, UpdateUserDTO DTO)
        {
            var user = await _userRepo.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update allowed fields
            if (DTO.FirstName != null)
            {
                user.FirstName = DTO.FirstName;
            }
            if (DTO.LastName != null)
            {
                user.LastName = DTO.LastName;
            }
            if (DTO.PhoneNr != null)
            {
                user.PhoneNr = DTO.PhoneNr;
            }
            if (DTO.FoodPreferences != null)
            {
                user.FoodPreferences = DTO.FoodPreferences;
            }
            if (!string.IsNullOrEmpty(DTO.Password))
            {
                if (!_passwordService.IsStrongPassword(DTO.Password))
                {
                    return BadRequest();
                }
                user.PasswordHash = _passwordService.HashPassword(DTO.Password);
            }
            if (DTO.Role.HasValue)
            {
                // Cast to Role from Role? is necessesary because Role must be mandatory in User
                user.Role = (Role)DTO.Role;
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
            _aws3Services = new Aws3Services(_appConfiguration.AwsAccessKey, _appConfiguration.AwsSecretAccessKey, _appConfiguration.Region, _appConfiguration.BucketName);

            var responseCV = _aws3Services.IfFileExists(user.Uuid + ".pdf");
            user.hasCv = responseCV;

            var responseProfilePicture = _aws3Services.IfFileExists(user.Id.ToString() + ".jpg");
            user.hasProfilePicture = responseProfilePicture;

            user.profilePictureUrl = "https://cvfiler.s3.eu-north-1.amazonaws.com/" + userId.ToString() + ".jpg";

            return Ok(user);
        }

        /// <summary>
        /// Update the signed in user's information
        /// </summary>
        [HttpPut]
        [Route("me")]
        [Authorize]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutMe(UpdateUserDTO DTO)
        {
            var userId = HttpContext.User.GetId();
            var user = await _userRepo.Get(userId);

            // Update allowed fields
            if (DTO.FirstName != null)
            {
                user.FirstName = DTO.FirstName;
            }
            if (DTO.LastName != null)
            {
                user.LastName = DTO.LastName;
            }
            if (DTO.PhoneNr != null)
            {
                user.PhoneNr = DTO.PhoneNr;
            }
            if (DTO.FoodPreferences != null)
            {
                user.FoodPreferences = DTO.FoodPreferences;
            }
            if (!string.IsNullOrEmpty(DTO.Password))
            {
                if (!_passwordService.IsStrongPassword(DTO.Password))
                {
                    return BadRequest();
                }
                user.PasswordHash = _passwordService.HashPassword(DTO.Password);
            }
            if (DTO.profilePictureUrl != null)
            {
                user.profilePictureUrl = DTO.profilePictureUrl;
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

