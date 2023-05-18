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
        private readonly PasswordService _passwordService;

        private IS3Configuration _appConfiguration;
        private IAws3Services _aws3Services;
        private IS3Configuration _s3Configuration;

        public RoleController(
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

            if (DTO.Role.HasValue)
            {
                // Cast to Role from Role? is necessesary because Role must be mandatory in User
                user.Role = (Role)DTO.Role;
            }

            await _userRepo.Update(user);

            return Ok(user);
        }

    }

}

