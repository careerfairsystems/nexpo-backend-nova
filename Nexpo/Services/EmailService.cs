using Nexpo.DTO;
using Nexpo.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using static Nexpo.DTO.FinalizeSignUpDTO;

namespace Nexpo.Services
{
    public abstract class IEmailService
    {
        protected readonly IConfig _config;
        protected readonly TokenService _tokenService;

        private string baseUrl = "https://nexpo-web.arkadtlth.se";

        public IEmailService(IConfig iConfig, TokenService tokenService)
        {
            _config = iConfig;
            _tokenService = tokenService;
        }

        protected abstract Task SendEmail(string recipient, string subject, string textContent, string htmlContent);

        public Task SendSignUpEmail(User user)
        {
            var signUpDTO = new FinalizeSignUpTokenDTO
            {
                UserId = user.Id.Value
            };
            var signedToken = _tokenService.SignToken(signUpDTO, DateTime.Now.AddDays(1)); // SignUp link is valid for one day
            var tokenString = Uri.EscapeDataString(signedToken);
            var content = $"Verify your email by clicking on this link: {baseUrl}/finalize_signup/{tokenString}";
            return SendEmail(user.Email, "Complete the signup", content, content);
        }

        public Task SendCompanyInviteEmail(Company company, User user)
        {
            var signUpDTO = new FinalizeSignUpTokenDTO
            {
                UserId = user.Id.Value
            }; 
            var signedToken = _tokenService.SignToken(signUpDTO, DateTime.Now.AddDays(7)); // SignUp link is valid for a week
            var tokenString = Uri.EscapeDataString(signedToken);
            var content =   $"Join your company on the Arkad fair by finalizing your account.<br><br>" 
                            + "This is needed to be able to connect with the studends during the fair with the Arkad-app<br>" 
                            + $"Click on the following link and set a password and you are good to go: {baseUrl}/finalize_signup/{tokenString}" 
                            + "<br><br>After you have finalized your account you need to download the app from the app store, search for \"arkad tlth\" and it should appear." 
                            + "<br><br>Should you have any further questions regarding this, the app or how to connect with students, feel free to contact us at it.arkad@tlth.se";
            return SendEmail(user.Email, "Join your company in the Arkad App", content, content);
        }

        public Task SendApplicationAcceptedEmail(Company company, User user)
        {
            var content =   "Congrats!<br>" 
                            + $"{company.Name} has just accepted your student session application!<br>" 
                            + "Log in to the app to pick a timeslot";
            return SendEmail(user.Email, $"{company.Name} accepted your application!", content, content);
        }

        public Task SendPasswordResetEmail(User user)
        {
            var passwordResetDTO = new ResetPasswordDTO.ResetPasswordTokenDTO
            {
                UserId = user.Id.Value
            };
            var signedToken = _tokenService.SignToken(passwordResetDTO, DateTime.Now.AddHours(1)); // Password reset link valid for an hour
            var tokenString = Uri.EscapeDataString(signedToken);
            var content = $"Reset your password by following this link: {baseUrl}/reset_password/{tokenString} The link is valid for an hour.";
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
