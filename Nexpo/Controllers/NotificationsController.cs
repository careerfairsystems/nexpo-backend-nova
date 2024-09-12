//using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Nexpo.DTO;
using Nexpo.Models;
using Nexpo.Repositories;
using System.Net.Http;
using System.Text;
using System.Text.Json;
namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserRepository _userRepo;

        public NotificationController(
            INotificationRepository iNotificationRepo,
            IUserRepository iUserRepo
            )
        {
            _notificationRepo = iNotificationRepo;
            _userRepo = iUserRepo;
        }
        
        private static Queue<NotificationDTO> history;

        static NotificationController()
        {
            history = new Queue<NotificationDTO>(10);
            history.Enqueue(new NotificationDTO { Title = "Welcome", Message = "Welcome to the app"});
        }

        /// <summary>
        /// Register a token to receive notifications of a specific topic
        /// 
        /// The standard topics are current "All" and "Volunteer"
        /// </summary>
        [HttpPost("register")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterUser(int id, RegisterUserDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token))
            {
                return BadRequest("Token is required.");
            }

            try
            {
                
                await _userRepo.AddToken(dto.Token, await _userRepo.Get(id));
            
                return Ok(new { success = true, detail = "Successfully registered for topic" });
                
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, detail = "An error occurred while registering for topic" });
            }
        }

        /// <summary>
        /// The api that the admin can use to send notifications to all users, 
        /// registerred to a specific topic
        /// 
        /// The standard topics are current "All" and "Volunteer"
        /// </summary>
        [HttpPut]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> Notify(NotificationDTO dto, Role role)
{

    if (string.IsNullOrEmpty(dto.Message))
    {
        return BadRequest("Message is required input.");
    }

    try
    {
        var client = new HttpClient();
        var payload = new
        {
            to = dto.Token,
            title = dto.Title,
            body = dto.Message,
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://exp.host/--/api/v2/push/send", content);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, detail = "An error occurred while sending notification" });
        }

        if (history.Count >= 10)
        {
            history.Dequeue();
        }
        history.Enqueue(dto);

        return Ok(new { success = true, detail = "Successfully sent notification" });
        
    }
    catch (Exception)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, detail = "An error occurred while sending notification" });
    }
}

        /// <summary>
        /// Returns the past few notifications
        /// At most 10 notifications will be stored at once
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetLastNotifications()
        {
            return Ok(history); 
        }

    }

}