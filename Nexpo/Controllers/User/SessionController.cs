using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;
        private readonly IEmailService _emailService;

        public SessionController(
            IUserRepository iUserRepo, 
            IStudentRepository iStudentRepo, 
            ICompanyRepository iCompanyRepo, 
            PasswordService passwordService,
            TokenService tokenService,
            IEmailService iEmailService)
        {
            _userRepo        = iUserRepo;
            _studentRepo     = iStudentRepo;
            _companyRepo     = iCompanyRepo;
            _passwordService = passwordService;
            _tokenService    = tokenService;
            _emailService    = iEmailService;
        }

        /// <summary>
        /// Sign in to application and get a JWT
        /// </summary>
        [HttpPost]
        [Route("signin")]
        [ProducesResponseType(typeof(SignInResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostSignIn(SignInRequestDto credentials)
        {
            // Force lowercase email
            credentials.Email = credentials.Email.ToLower();

            var user = await _userRepo.FindByEmail(credentials.Email);
            if (user == null)
            {
                return BadRequest();
            }

            if (!_passwordService.ValidatePassword(credentials.Password, user.PasswordHash))
            {
                return BadRequest();
            }

            // Common claims
            var claims = new List<Claim>
            {
                new Claim(UserClaims.Id, user.Id.ToString()),
                new Claim(UserClaims.Role, user.Role.ToString()),
            };

            if (user.Role == Role.Student)
            {
                var student = await _studentRepo.FindByUser(user.Id.Value);
                claims.Add(new Claim(UserClaims.StudentId, student.Id.ToString()));
            }

            if (user.Role == Role.CompanyRepresentative)
            {
                var company = await _companyRepo.FindByUser(user.Id.Value);
                claims.Add(new Claim(UserClaims.CompanyId, company.Id.ToString()));
            }

            var jwt = _tokenService.GenerateJWT(claims);

            return Ok(new SignInResponseDto { Token = jwt });
        }

        [HttpPost]
        [Route("forgot_password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _userRepo.FindByEmail(dto.Email);
            // Don't expose account existance
            if (user != null)
            {
                // Don't await the email to prevent timing attacks
                _ = _emailService.SendPasswordResetEmail(user);
            }

            return NoContent();
        }

        [HttpPost]
        [Route("reset_password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PostResetPassword(ResetPasswordDto dto)
        {
            dto.Token = Uri.UnescapeDataString(dto.Token);
            var token = _tokenService.ValidateToken<ResetPasswordDto.ResetPasswordTokenDto>(dto.Token);
            if (!token.IsValid)
            {
                return Forbid();
            }

            if (!_passwordService.IsStrongPassword(dto.Password))
            {
                return BadRequest();
            }

            var userId = token.Value.UserId;
            var user = await _userRepo.Get(userId);
            user.PasswordHash = _passwordService.HashPassword(dto.Password);
            await _userRepo.Update(user);

            return NoContent();
        }
    }

    /// <summary>
    /// The claimes a signed in user can have
    /// </summary>
    public static class UserClaims
    {
        public static readonly string Id = nameof(Id);
        public static readonly string Role = ClaimTypes.Role; 
        public static readonly string CompanyId = nameof(CompanyId);
        public static readonly string StudentId = nameof(StudentId);
    }
}

