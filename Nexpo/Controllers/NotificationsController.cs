using FirebaseAdmin.Messaging;
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

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public NotificationController()
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("Fill/in/json-adress here"),
            });
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
                var messaging = FirebaseMessaging.DefaultInstance;

                var registrationTokens = new List<string> { dto.Token };

                TopicManagementResponse response = await messaging.SubscribeToTopicAsync(registrationTokens, dto.Topic);

                return Ok(new { success = true, detail = "Successfully registered for topic" });
            }
            catch (Exception ex)
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
                return Ok(new { success = true, detail = "Successfully sent notification" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, detail = "An error occurred while sending notification" });
            }
        }

    }

}