using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO.Notifications;
using Nexpo.Models;
using Nexpo.Services;
using Nexpo.Repositories;
using Microsoft.AspNetCore.Http;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly IUserRepository _userRepo;
        private readonly ITicketRepository _ticketRepo;

        public NotificationsController(NotificationService notificationService, IUserRepository userRepo, ITicketRepository ticketRepo)
        {
            _notificationService = notificationService;
            _userRepo = userRepo;
            _ticketRepo = ticketRepo;
        }

        /// <summary>
        /// Create a new notification
        /// </summary>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateNotification(AddNotificationDTO dto)
        {
            var notification = new Notification
            {
                Message = dto.Message,
                ScheduledTime = dto.ScheduledTime,
                NotificationType = dto.NotificationType,
                EventId = dto.EventId
            };

            await _notificationService.CreateNotificationAsync(notification);

            // If the notification has no scheduled time, send it now
            if (notification.ScheduledTime == null)
            {
                await SendNotifications(notification, dto.Title);
            }

            return Ok(notification);
        }

        /// <summary>
        /// Send a notification manually
        /// </summary>
        [HttpPost]
        [Route("send/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SendNotification(int id, [FromBody] NotificationDTO dto)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            await SendNotifications(notification, dto.Title);
            return Ok();
        }

        /// <summary>
        /// Private method to send notifications based on NotificationType
        /// </summary>
        private async Task SendNotifications(Notification notification, string title)
        {
            string defaultTitle = string.IsNullOrEmpty(title) ? "Notification" : title;

            switch (notification.NotificationType)
            {
                case NotificationType.General:
                    var allUsers = await _userRepo.GetAll();
                    foreach (var user in allUsers)
                    {
                        await _notificationService.SendNotificationToUser(notification.Id, user.Id.Value, defaultTitle);
                    }
                    break;

                case NotificationType.EventReminder:
                    if (notification.EventId.HasValue)
                    {
                        var eventUsers = await _ticketRepo.GetAllUsersForEvent(notification.EventId.Value);
                        foreach (var user in eventUsers)
                        {
                            await _notificationService.SendNotificationToUser(notification.Id, user.Id.Value, defaultTitle);
                        }
                    }
                    break;

                case NotificationType.RoleBased:
                    var roleUsers = await _userRepo.GetUsersByRole(Role.Volunteer);
                    foreach (var user in roleUsers)
                    {
                        await _notificationService.SendNotificationToUser(notification.Id, user.Id.Value, defaultTitle);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
