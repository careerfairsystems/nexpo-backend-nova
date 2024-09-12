using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Nexpo.Repositories
{
    public interface INotificationRepository
    {
        public Task<Notification> Get(int id);
        public Task Add(Notification _notification);
        public Task Update(Notification _notification);
        public Task Remove(Notification _notification);
        public Task<List<Notification>> GetAllNotExpired(DateTime date, Notification.NotificationType type);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Notification> Get(int id)
        {
            return _context.Notification.FirstOrDefaultAsync(_notification => _notification.Id == id);
        }
        public async Task Add(Notification _notification)
        {
            _context.Notification.Add(_notification);
            await _context.SaveChangesAsync();
        }
        public async Task Remove(Notification _notification)
        {
            _context.Notification.Remove(_notification);
            await _context.SaveChangesAsync();
        }
        public Task Update(Notification _notification){
            _context.Entry(_notification).State = EntityState.Modified;
            return _context.SaveChangesAsync();

        }
        public async Task<List<Notification>> GetAllNotExpired(DateTime date, Notification.NotificationType type)
        {
            return await _context.Notification.Where(_notification => _notification.ScheduledTime > date && _notification.Type == type).ToListAsync();
        } 
        



        
        






    }
}
