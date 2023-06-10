using FirebaseAdmin.Messaging;

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
            var message = new MulticastMessage()
            {
                Notification = new Notification
                {
                    Title = "Notification Title",
                    Body = "Notification Body",
                },
                Topic = "all",
            };

            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

        }

    }

}