using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;

namespace Nexpo.Controllers
{
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IEmailService _emailService;
        private readonly TokenService _tokenService;
        private readonly PasswordService _passwordService;

        public SignUpController(IUserRepository iUserRepo, 
            ICompanyRepository iCompanyRepo, 
            IStudentRepository iStudentRepo, 
            IEmailService emailService,
            TokenService tokenService,
            PasswordService passwordService)
        {
            _userRepo        = iUserRepo;
            _companyRepo     = iCompanyRepo;
            _studentRepo     = iStudentRepo;
            _emailService    = emailService;
            _tokenService    = tokenService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Initiate a student signup.
        /// </summary>
        [HttpPost]
        [Route("initial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostInitialSignUp(SignUpUserDTO DTO)
        {
            // Force lowercase email
            DTO.Email = DTO.Email.ToLower();

            var user = await _userRepo.FindByEmail(DTO.Email);
            if (user != null)
            {
                return Conflict();
            }

            user = new User
            {
                Role      = Role.Student,
                Email     = DTO.Email,
                FirstName = DTO.FirstName,
                LastName  = DTO.LastName
            };
            await _userRepo.Add(user);

            var student = new Student
            {
                UserId = user.Id.Value
            };
            await _studentRepo.Add(student);

            await _emailService.SendSignUpEmail(user);

            return NoContent();
        }

        /// <summary>
        /// Finalize a user signup from a signup token
        /// </summary>
        [HttpPost]
        [Route("finalize")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostFinalizedSignUp(FinalizeSignUpDTO DTO)
        {
            var token = _tokenService.ValidateToken<FinalizeSignUpDTO.FinalizeSignUpTokenDTO>(DTO.Token);
            if (!token.IsValid)
            {
                return Forbid();
            }

            // Check password strength
            if (!_passwordService.IsStrongPassword(DTO.Password))
            {
                return BadRequest();
            }

            var userId = token.Value.UserId;
            var user = await _userRepo.Get(userId);
            var passwordHash = _passwordService.HashPassword(DTO.Password);
            user.PasswordHash = passwordHash;
            await _userRepo.Update(user);

            return NoContent();
        }

        /// <summary>
        /// Invite a representative to become a new user (connected to a company)
        [HttpPost]
        [Route("representative")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostInviteRepresentative(InviteRepresentativeDTO DTO)
        {
            var userRole = HttpContext.User.GetRole();
            if (userRole == Role.CompanyRepresentative)
            {
                var userCompanyId = HttpContext.User.GetCompanyId().Value;
                if (DTO.CompanyId != userCompanyId)
                {
                    return Forbid();
                }
            }

            var user = await _userRepo.FindByEmail(DTO.Email);
            if (user != null)
            {
                return Conflict();
            }

            var company = await _companyRepo.Get(DTO.CompanyId);
            if (company == null)
            {
                return NotFound();
            }

            user = new User
            {
                Role      = Role.CompanyRepresentative,
                Email     = DTO.Email,
                FirstName = DTO.FirstName,
                LastName  = DTO.LastName,
                CompanyId = company.Id.Value
            };
            
            await _userRepo.Add(user);

            await _emailService.SendCompanyInviteEmail(company, user);

            return NoContent();
        }
        
    }
}

