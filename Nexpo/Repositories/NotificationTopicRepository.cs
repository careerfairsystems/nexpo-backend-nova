using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nexpo.Models.NotificationTopic; //Denna borde ha importerats från Nexpo.Models?

namespace Nexpo.Repositories
{
    public interface INotificationTopicRepository
    {
        //public Task<IEnumerable<Event>> GetAll();
        public Task<List<NotificationTopic>> GetUserTopics(User _user);
        public Task<List<User>> GetTopicSubscribers(NotificationTopic _topic);
        public Task Add(TopicType _topic, User user);
        public Task Delete(NotificationTopic _notificationTopic);
    }

    public class NotificationTopicRepository : INotificationTopicRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationTopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        /*
        public async Task<IEnumerable<Event>> GetAll()
        {
            return await _context.Events.OrderBy(_event => _event.Date)
                                        .ThenBy(_event => _event.Start)
                                        .Select(_event => new Event
                                        {
                                            Id = _event.Id,
                                            Name = _event.Name,
                                            Description = _event.Description,
                                            Date = _event.Date,
                                            Type = _event.Type,
                                            Start = _event.Start,
                                            End = _event.End,
                                            Location = _event.Location,
                                            Host = _event.Host,
                                            Language = _event.Language,
                                            Capacity = _event.Capacity,
                                            TicketCount = _event.Tickets.Count()
                                        })
                                        .ToListAsync();
        }
*/


        public async Task<List<NotificationTopic>> GetUserTopics(User user)
        {
            return await _context.NotificationTopic.Where(_topic => _topic.User == user).ToListAsync();
        }
        public async Task<List<User>> GetTopicSubscribers(NotificationTopic topic)
        {
            return await _context.NotificationTopic.Where(_topic => _topic.Topic == topic.Topic).Select(_topic => _topic.User).ToListAsync();
        }
        

        public async Task Add(TopicType topic, User user)
        {
            _context.NotificationTopic.Add(new NotificationTopic { Topic = topic, User = user });
            await _context.SaveChangesAsync();
        }


        public async Task Delete(NotificationTopic NotificationTopic)
        {
            _context.NotificationTopic.Remove(NotificationTopic);
            await _context.SaveChangesAsync();
        }
    }
}
