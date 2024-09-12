using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexpo.Models;
using Nexpo.Services;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            return Ok(await _notificationService.GetAllNotificationsAsync());
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        // POST: api/Notifications
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notification)
        {
            await _notificationService.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }

        // PUT: api/Notifications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, Notification notification)
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            try
            {
                await _notificationService.UpdateNotificationAsync(notification);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _notificationService.GetNotificationByIdAsync(id) == null)
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Notifications/User/5
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(int userId)
        {
            return Ok(await _notificationService.GetUserNotificationsAsync(userId));
        }

        // POST: api/Notifications/Subscribe
        [HttpPost("Subscribe")]
        public async Task<IActionResult> SubscribeUserToNotification(int userId, int notificationId)
        {
            await _notificationService.SubscribeUserToNotificationAsync(userId, notificationId);
            return NoContent();
        }

        // POST: api/Notifications/Unsubscribe
        [HttpPost("Unsubscribe")]
        public async Task<IActionResult> UnsubscribeUserFromNotification(int userId, int notificationId)
        {
            await _notificationService.UnsubscribeUserFromNotificationAsync(userId, notificationId);
            return NoContent();
        }

        // POST: api/Notifications/Schedule
        [HttpPost("Schedule")]
        public async Task<IActionResult> ScheduleNotification(Notification notification, DateTime scheduledTime)
        {
            await _notificationService.ScheduleNotificationAsync(notification, scheduledTime);
            return NoContent();
        }
    }
}
