using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;


namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        /// <summary>
        /// The api that the admin can use to send notifications to all users
        /// </summary>
        [HttpPut]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> NotifyAll(NotificationDTO dto)
        {
            // console log

            if(String.IsNullOrEmpty("title") || String.IsNullOrEmpty("message"))
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

                Topic = "all"
            };


            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            Console.WriteLine(result); //projects/myapp/messages/2492588335721724324

            

            return Ok();

        }

    }

}