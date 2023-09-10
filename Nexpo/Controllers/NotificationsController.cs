using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth.OAuth2;
using Nexpo.DTO;
using Nexpo.Models;
using FirebaseAdmin;

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
                Credential = GoogleCredential.FromFile("path/to/your/firebase/credentials.json"),
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

            var app = FirebaseApp.DefaultInstance ?? FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("path/to/your/firebase/credentials.json"),
            });

            string topic = dto.Topic;

            var messaging = FirebaseMessaging.DefaultInstance;
            var registrationTokens = new List<string>
            {
                dto.Token
            };

            TopicManagementResponse response = await messaging.SubscribeToTopicAsync(registrationTokens, topic);

            return Ok();
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
                return BadRequest("Topic is required.");
            }

            if (String.IsNullOrEmpty(dto.Message))
            {
                return BadRequest();
            }

            var message = new Message()
            {

                Notification = new Notification
                {
                    Title = dto.Title,
                    Body = dto.Message,
                },

                Topic = dto.Topic
            };

            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);

            return Ok();

        }

    }

}