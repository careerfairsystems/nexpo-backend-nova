using Nexpo.DTO;
using Nexpo.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using static Nexpo.DTO.FinalizeSignUpDto;

namespace Nexpo.Services
{
    public abstract class IEmailService
    {
        protected readonly IConfig _config;
        protected readonly TokenService _tokenService;

        public IEmailService(IConfig iConfig, TokenService tokenService)
        {
            _config = iConfig;
            _tokenService = tokenService;
        }

        protected abstract Task SendEmail(string recipient, string subject, string textContent, string htmlContent);

        public Task SendSignUpEmail(User user)
        {
            var signUpDto = new FinalizeSignUpTokenDto
            {
                UserId = user.Id.Value
            };
            var signedToken = _tokenService.SignToken(signUpDto, DateTime.Now.AddDays(1)); // SignUp link is valid for one day
            var tokenString = Uri.EscapeDataString(signedToken);
            var content = $"Verify your email by clicking on this link: {_config.BaseUrl}/finalize_signup/{tokenString}";
            return SendEmail(user.Email, "Complete the signup", content, content);
        }

        public Task SendCompanyInviteEmail(Company company, User user)
        {
            var signUpDto = new FinalizeSignUpTokenDto
            {
                UserId = user.Id.Value
            }; 
            var signedToken = _tokenService.SignToken(signUpDto, DateTime.Now.AddDays(7)); // SignUp link is valid for a week
            var tokenString = Uri.EscapeDataString(signedToken);
            var content = $"Join your company account by clicking on this link: {_config.BaseUrl}/finalize_signup/{tokenString}";
            return SendEmail(user.Email, "Join your company workspace", content, content);
        }

        public Task SendPasswordResetEmail(User user)
        {
            var passwordResetDto = new ResetPasswordDto.ResetPasswordTokenDto
            {
                UserId = user.Id.Value
            };
            var signedToken = _tokenService.SignToken(passwordResetDto, DateTime.Now.AddHours(1)); // Password reset link valid for an hour
            var tokenString = Uri.EscapeDataString(signedToken);
            var content = $"Join your company account by clicking on this link: {_config.BaseUrl}/reset_password/{tokenString}";
            return SendEmail(user.Email, "Reset your password", content, content);
        }
    }

    public class EmailService : IEmailService
    {
        public EmailService(IConfig iConfig, TokenService tokenService) : base(iConfig, tokenService)
        {
        }

        protected override async Task SendEmail(string recipient, string subject, string textContent, string htmlContent)
        {
            var apiKey = _config.SendGridApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ne-reply@arkadtlth.se", "Nexpo Arkad");
            var to = new EmailAddress(recipient);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, textContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }

    public class DevEmailService : IEmailService
    {
        public DevEmailService(IConfig iConfig, TokenService tokenService) : base(iConfig, tokenService)
        {
        }

        protected override Task SendEmail(string recipient, string subject, string textContent, string htmlContent)
        {
            Console.WriteLine();
            Console.WriteLine("=================  SENDING EMAIL  =================");
            Console.WriteLine($"Recipient:  {recipient}                            ");
            Console.WriteLine($"Subject:    {subject}                              ");
            Console.WriteLine("=================  HTML CONTENT  ==================");
            Console.WriteLine(textContent);
            Console.WriteLine("=================  TEXT CONTENT  ==================");
            Console.WriteLine(htmlContent);
            Console.WriteLine("======================  END  ======================");
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
