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
            history.Enqueue(new NotificationDTO { Title = "Welcome", Message = "Welcome to the app", Topic = "all" });
        }

        /// <summary>
        /// Register a token to receive notifications of a specific topic
        /// 
        /// The standard topics are current "All" and "Volunteer"
        /// </summary>
        [HttpPost("register")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token))
            {
                return BadRequest("Token is required.");
            }

            try
            {
                
                await _userRepo.AddToken(dto.Token, await _userRepo.Get(dto.UserId));
            
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
        public async Task<IActionResult> NotifyAll(NotificationDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Topic))
            {
                return BadRequest("Topic is required input.");
            }

            if (string.IsNullOrEmpty(dto.Message))
            {
                return BadRequest("Message is required input.");
            }

            try
            {
                var messaging = FirebaseMessaging.DefaultInstance;
                var message = new Message
                {
                    Notification = new Notification { Title = dto.Title, Body = dto.Message },
                    Topic = dto.Topic
                };

                var result = await messaging.SendAsync(message);

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