using Nexpo.DTO;
using Nexpo.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
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

        // public Task SendCompanyInviteEmail(Company company, User user)
        // {
        //     var signUpDTO = new FinalizeSignUpTokenDTO
        //     {
        //         UserId = user.Id.Value
        //     }; 
        //     var signedToken = _tokenService.SignToken(signUpDTO, DateTime.Now.AddDays(7)); // SignUp link is valid for a week
        //     var tokenString = Uri.EscapeDataString(signedToken);
        //     var content =   $"Join your company on the ARKAD fair by finalizing your account.<br><br>" 
        //                     + "This is needed to be able to connect with the studends during the fair, through the ARKAD student sessions website or ARKAD app<br>" 
        //                     + $"Click on the following link and set a password and you are good to go: {baseUrl}/finalize_signup/{tokenString}" 
        //                     + "<br><br> After finalizing your account, you have access to log into https://nexpo-web.arkadtlth.se"
        //                     + "The same log in also works in the ARKAD app: search for \"arkad tlth\" in your app store." 
        //                     + "<br><br>Should you have any further questions regarding this, the app or how to connect with students, feel free to contact us at it.arkad@tlth.se";
        //     return SendEmail(user.Email, "Join your company in the ARKAD Website And App", content, content);
        // }


        public Task SendCompanyInviteEmail(Company company, User user)
        {
            var signUpDTO = new FinalizeSignUpTokenDTO
            {
                UserId = user.Id.Value
            }; 
            var signedToken = _tokenService.SignToken(signUpDTO, DateTime.Now.AddDays(7)); // SignUp link is valid for a week
            var tokenString = Uri.EscapeDataString(signedToken);
            var content =   $"Dear {company.Name},<br>" 
                            + $"Thank you for choosing to participate in the Student Sessions. This email aims to furnish you with essential details concerning your preparation and engagement"
                            + $"To begin, please create your account via the following link: {baseUrl}/finalize_signup/{tokenString}. <br><br>"
                            + "The primary step will be to set your password. Once your account is established, kindly log into the Student Sessions portal at https://nexpo-web.arkadtlth.se"
                            + " Ensure you use the same email address to which this message was sent. Within the portal, you'll have the capability to view, approve, or reject applications."
                            + "Upon approval, the schedule for your confirmed meetings will be accessible. If needed, a video tutorial within the portal is available for your assistance.<br><br>"
                            + "Key Dates to Remember:<br>" 
                            + " * Student application window: 23rd October to 5th November.<br>"
                            + " * Company selection timeframe: 6th November to 10th November.<br><br>"
                            + "An attached map, included in this email, marks your company's designated location within E-house for the Student Sessions. The room designated for you will be ready and set upon your arrival."
                            + " If you have any specific room requirements or requests, please contact adam.shafiei@users.tlth.se."
                            + "Hosts will routinely check to determine if you require any water or food from the lounge.<br>"
                            + "Please be advised, this is a no-reply email. For any inquiries or further details, reach out to company.arkad@tlth.se.Warm regards,<br><br>"
                            + "Niklas Ku, Business Manager 2023";
            return SendEmail(user.Email, "ARKAD Student Sessions Instructions", content, content);
        }

        public Task SendVolunteerInviteEmail(User user)
        {
            var signUpDTO = new FinalizeSignUpTokenDTO
            {
                UserId = user.Id.Value
            }; 

            var signedToken = _tokenService.SignToken(signUpDTO, DateTime.Now.AddDays(7));
            var tokenString = Uri.EscapeDataString(signedToken);
            var content =   $"Join your other ARKAD volunteers in the ARKAD app by finalizing your account.<br><br>" 
                            + "This is needed to be able to connect with the studends during the fair with the ARKAD-app<br>" 
                            + $"Click on the following link and set a password and you are good to go: {baseUrl}/finalize_signup/{tokenString}" 
                            + "<br><br>After you have finalized your account you need to download the app from the app store, search for \"arkad tlth\" and it should appear." 
                            + "<br><br>Should you have any further questions regarding this, feel free to contact us at it.arkad@tlth.se";
            return SendEmail(user.Email, "Join the ARKAD App", content, content);
        }

        public Task SendApplicationPendingEmail(Company company, User user)
        {
            var content =   "Application successfully sent in!<br>" 
                            + $"{company.Name} has received your student session application!<br>" 
                            + "You will receive an email when your application has been accepted or rejected.";
            return SendEmail(user.Email, $"{company.Name} have received your application!", content, content);
        }

        public Task SendApplicationRejectedEmail(Company company, User user)
        {
            var content =   "Your application have been rejected.<br>" 
                            + $"{company.Name} has just rejected your student session application.<br>" 
                            + "Better luck next time!";
            return SendEmail(user.Email, $"{company.Name} rejected your application!", content, content);
        }

        public Task SendApplicationAcceptedEmail(Company company, User user)
        {
            var content =   "Congrats!<br>" 
                            + $"{company.Name} has just accepted your student session application!<br>" 
                            + $"Log in to the app and choose {company.Name} in Student Sessions to pick a timeslot";
            return SendEmail(user.Email, $"{company.Name} rejected your application!", content, content);
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


        public Task SendTicketAsQRViaEmail(string targetMail, Guid ticketId, Event _event, string appearAt)
        {
            var name = _event.Name;
            var location = _event.Location;
            var host = _event.Host;

            var date = _event.Date;
            var start = _event.Start;
            var end = _event.End;

            string qrImage = "https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=" + ticketId;

            var content = $"You have received an invitation for the event: {name}, located at {location}, scheduled for {date}";
            
            if(string.IsNullOrEmpty(appearAt)){
                content += $", between {start} and {end}.<br><br>";
            }else{
                content += ". ";
                content += appearAt;
                content += "<br><br>";
            }
            
            content += $"Please show the QR-code below at the entrance to get in.<br><br>" + qrImage;

            return SendEmail(targetMail, $"Arkad Ticket for {name}", content, content);

        }

        public Task SendTicketAsQRViaEmail(string targetMail, List<Ticket> tickets, Event _event, string appearAt)
        {
            var name = _event.Name;
            var location = _event.Location;
            var host = _event.Host;

            var date = _event.Date;
            var start = _event.Start;
            var end = _event.End;

            var numberOfTickets = tickets.Count;

            string qrImage = "https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=";

            var content = $"You and your {numberOfTickets-1} collegue(s) have received an invitation for the event: {name}, located at {location}, scheduled for {date}";
            
            if(string.IsNullOrEmpty(appearAt)){
                content += $", between {start} and {end}.<br><br>";
            }else{
                content += ". ";
                content += appearAt;
                content += "<br><br>";
            }
        
            content += $"Please show the QR-codes below at the entrance to get in.<br><br>";

            foreach (var ticket in tickets)
            {
                content += qrImage+ticket.Code+"<br><br>";
                //content += $"<img src=\"{qrImage}{ticket.Code}\" alt=\"QR-code\" width=\"300\" height=\"300\">";
            }

            return SendEmail(targetMail, $"ARKAD Tickets for {name}", content, content);
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
