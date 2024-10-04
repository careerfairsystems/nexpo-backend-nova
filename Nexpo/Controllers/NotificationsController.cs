using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO.Notifications;
using Nexpo.Models;
using Nexpo.Services;
using Nexpo.Repositories;

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
        //[Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateNotification(AddNotificationDTO DTO)
        {
            var notification = new Notification
            {
                Message = DTO.Message,
                ScheduledTime = DTO.ScheduledTime,
                NotificationType = DTO.NotificationType,
                EventId = DTO.EventId
            };

            await _notificationService.CreateNotificationAsync(notification);

            // If the notification is has no scheduled time, send it now
            if (notification.ScheduledTime == null)
            {
                await SendNotifications(notification);
            }

            return Ok(notification);
        }

        /// <summary>
        /// Send a notification manually
        /// </summary>
        [HttpPost]
        [Route("send/{id}")]
        //[Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SendNotification(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            await SendNotifications(notification);
            return Ok();
        }

        /// <summary>
        /// Subscribe a user to event notifications
        /// </summary>
        [HttpPost]
        [Route("subscribe")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SubscribeToEvent(SubscribeNotificationDTO DTO)
        {
            await _notificationService.SubscribeUserToEventAsync(DTO.UserId, DTO.EventId);
            return Ok();
        }

        /// <summary>
        /// Unsubscribe a user from event notifications
        /// </summary>
        [HttpPost]
        [Route("unsubscribe")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UnsubscribeFromEvent(SubscribeNotificationDTO DTO)
        {
            await _notificationService.UnsubscribeUserFromEventAsync(DTO.UserId, DTO.EventId);
            return Ok();
        }

        /// <summary>
        /// Get notifications for a specific user
        /// </summary>
        [HttpGet]
        [Route("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            var notificationDTOs = notifications.Select(notification => new NotificationDTO
            {
                Id = notification.Id,
                Message = notification.Message,
                ScheduledTime = notification.ScheduledTime,
                NotificationType = notification.NotificationType,
                EventId = notification.EventId
            });

            return Ok(notificationDTOs);
        }

        /// <summary>
        /// Private method to send notifications based on NotificationType
        /// </summary>
        private async Task SendNotifications(Notification notification)
        {
            switch (notification.NotificationType)
            {
                case NotificationType.General:
                    // Send to all users
                    var allUsers = await _userRepo.GetAll();
                    foreach (var user in allUsers)
                    {
                        await _notificationService.SendNotificationToUser(notification.Id, user.Id.Value);
                    }
                    break;

                case NotificationType.EventReminder:
                    // Send to users subscribed to the event
                    if (notification.EventId.HasValue)
                    {
                        var eventUsers = await _ticketRepo.GetAllUsersForEvent(notification.EventId.Value);
                        foreach (var user in eventUsers)
                        {
                            await _notificationService.SendNotificationToUser(notification.Id, user.Id.Value);
                        }
                    }
                    break;

                case NotificationType.RoleBased:
                    // Send to users based on role
                    var roleUsers = await _userRepo.GetUsersByRole(Role.Volunteer);
                    foreach (var user in roleUsers)
                    {
                        await _notificationService.SendNotificationToUser(notification.Id, user.Id.Value);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}