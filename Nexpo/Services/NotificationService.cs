using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;


namespace Nexpo.Services
{
    public class NotificationService
    {
        private readonly IUserRepository _userRepo;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public NotificationService(IUserRepository userRepo, HttpClient httpClient, ApplicationDbContext context)
        {
            _userRepo = userRepo;
            _httpClient = httpClient;
            _context = context;
        }

        public async Task SendNotificationToUser(int notificationId, int userId, string title)
        {
            var user = await _userRepo.Get(userId);
            if (user == null || string.IsNullOrEmpty(user.ExpoPushToken))
            {
                // User does not exist or has no Expo Push Token
                return;
            }

            var notification = await GetNotificationByIdAsync(notificationId);
            if (notification == null)
            {
                return;
            }

            // Create the notification payload
            var notificationPayload = new
            {
                to = user.ExpoPushToken,
                title = title,
                body = notification.Message
            };

            // Convert the payload to JSON
            var jsonPayload = JsonConvert.SerializeObject(notificationPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Add headers to the request
            var request = new HttpRequestMessage(HttpMethod.Post, "https://exp.host/--/api/v2/push/send")
            {
                Content = content
            };

            request.Headers.Add("accept", "application/json");
            request.Headers.Add("accept-encoding", "gzip, deflate");
            request.Headers.Add("host", "exp.host");

            // Send the request to Expo's API
            var response = await _httpClient.SendAsync(request);
            var resultContent = await response.Content.ReadAsStringAsync();

            // Log the response
            Console.WriteLine($"Response: {response}");
            Console.WriteLine($"Response Content: {resultContent}");
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();
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
