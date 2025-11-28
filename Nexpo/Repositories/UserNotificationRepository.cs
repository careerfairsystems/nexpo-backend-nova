using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Nexpo.Repositories
{
    public interface IUserNotificationRepository
    {
        public Task<UserNotification> Get(int userId, int notificationId);
        public Task Add(UserNotification _userNotification);
        public Task Update(UserNotification _userNotification);
        public Task Remove(UserNotification _userNotification);
        
    }

    public class UserNotificationRepository : IUserNotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public UserNotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<UserNotification> Get(int userId, int notificationId)
        {
            return _context.UserNotification.FirstOrDefaultAsync(_userNotification => _userNotification.UserId == userId 
            && _userNotification.NotificationId == notificationId);
        }
        public async Task Add(UserNotification _userNotification)
        {
            _context.UserNotification.Add(_userNotification);
            await _context.SaveChangesAsync();
        }
        public async Task Remove(UserNotification _userNotification)
        {
            _context.UserNotification.Remove(_userNotification);
            await _context.SaveChangesAsync();
        }
        
        public Task Update(UserNotification _userNotification){
            _context.Entry(_userNotification).State = EntityState.Modified;
            return _context.SaveChangesAsync();

        }


    }
}
