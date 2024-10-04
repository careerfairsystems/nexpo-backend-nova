using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nexpo.Models;

namespace Nexpo.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task SendNotificationToUser(int notificationId, int userId)
        {
            var userNotification = new UserNotification
            {
                NotificationId = notificationId,
                UserId = userId
            };

            _context.UserNotifications.Add(userNotification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<Notification> UpdateNotificationAsync(Notification notification)
        {
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return false;
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SubscribeUserToEventAsync(int userId, int eventId)
        {
            var userNotification = new UserNotification
            {
                UserId = userId,
                NotificationId = eventId
            };

            _context.UserNotifications.Add(userNotification);
            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeUserFromEventAsync(int userId, int eventId)
        {
            var userNotification = await _context.UserNotifications
                .FirstOrDefaultAsync(un => un.UserId == userId && un.NotificationId == eventId);

            if (userNotification != null)
            {
                _context.UserNotifications.Remove(userNotification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
        {
            return await _context.UserNotifications
                .Where(un => un.UserId == userId)
                .Select(un => un.Notification)
                .ToListAsync();
        }
    }
}
