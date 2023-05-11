using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using Nexpo.DTO;
using Nexpo.Models;

using AspNetCoreHero.ToastNotification.Abstractions;



namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        //THIS CLASS REALLY NEEDS TO BE TESTED TO BE WORKING ON THE PHONE

        private readonly INotyfService _notyf;
        public NotificationController(INotyfService notyf)
        {
            _notyf = notyf;
        }



        /// <summary>
        /// The api that the admin can use to send notifications to all users
        /// </summary>
        [HttpPut]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> NotifyAll(NotificationDTO dto){
            _notyf.Information(dto.Message, 5);
            return Ok();

        }

        /// <summary>
        /// The api that the user can use to get all notifications (that have been received)
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(){
            var notifications = _notyf.GetNotifications();

            var notificationDTOs = from notis in notifications
                select new NotificationDTO{
                    Message = notis.Message,

                };


            return Ok(notificationDTOs);
        }

        /// <summary>
        /// The api that the user can use to get the latest N notifications
        /// </summary>
        [HttpGet]
        [Route("latest/{N}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLatest(int N){
            var notifications = _notyf.GetNotifications();

            if(notifications.Count() < N){
                return NotFound();
            }

            var latestN = notifications.TakeLast(N);

            var notificationDTOs = from notis in latestN
                select new NotificationDTO{
                    Message = notis.Message,

                };

            return Ok(latestN);
        }



        
    }
    
}

