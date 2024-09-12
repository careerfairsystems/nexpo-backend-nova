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

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
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

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.UserNotifications
                .Where(un => un.UserId == userId)
                .Select(un => un.Notification)
                .ToListAsync();
        }

        public async Task SubscribeUserToNotificationAsync(int userId, int notificationId)
        {
            var userNotification = new UserNotification
            {
                UserId = userId,
                NotificationId = notificationId
            };

            _context.UserNotifications.Add(userNotification);
            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeUserFromNotificationAsync(int userId, int notificationId)
        {
            var userNotification = await _context.UserNotifications
                .FirstOrDefaultAsync(un => un.UserId == userId && un.NotificationId == notificationId);

            if (userNotification != null)
            {
                _context.UserNotifications.Remove(userNotification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ScheduleNotificationAsync(Notification notification, DateTime scheduledTime)
        {
            // Logic to schedule notification, e.g., using a background task or external service
        }
    }
}