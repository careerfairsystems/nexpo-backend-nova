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
            _userRepo = iUserRepo;
            _companyRepo = iCompanyRepo;
            _studentRepo = iStudentRepo;
            _emailService = emailService;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Initiate a student signup.
        /// </summary>
        [HttpPost]
        [Route("initial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostInitialSignUp(SignUpUserDto dto)
        {
            // Force lowercase email
            dto.Email = dto.Email.ToLower();

            var user = await _userRepo.FindByEmail(dto.Email);
            if (user != null)
            {
                return Conflict();
            }

            user = new User
            {
                Role = Role.Student,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
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
        public async Task<ActionResult> PostFinalizedSignUp(FinalizeSignUpDto dto)
        {
            var token = _tokenService.ValidateToken<FinalizeSignUpDto.FinalizeSignUpTokenDto>(dto.Token);
            if (!token.IsValid)
            {
                return Forbid();
            }

            // Check password strength
            if (!_passwordService.IsStrongPassword(dto.Password))
            {
                return BadRequest();
            }

            var userId = token.Value.UserId;
            var user = await _userRepo.Get(userId);
            var passwordHash = _passwordService.HashPassword(dto.Password);
            user.PasswordHash = passwordHash;
            await _userRepo.Update(user);

            return NoContent();
        }

        [HttpPost]
        [Route("representative")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostInviteRepresentative(InviteRepresentativeDto dto)
        {
            var userRole = HttpContext.User.GetRole();
            if (userRole == Role.CompanyRepresentative)
            {
                var userCompanyId = HttpContext.User.GetCompanyId().Value;
                if (dto.CompanyId != userCompanyId)
                {
                    return Forbid();
                }
            }

            var user = await _userRepo.FindByEmail(dto.Email);
            if (user != null)
            {
                return Conflict();
            }

            var company = await _companyRepo.Get(dto.CompanyId);
            if (company == null)
            {
                return NotFound();
            }

            user = new User
            {
                Role = Role.CompanyRepresentative,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CompanyId = company.Id.Value
            };
            await _userRepo.Add(user);

            await _emailService.SendCompanyInviteEmail(company, user);

            return NoContent();
        }
        
    }
}

