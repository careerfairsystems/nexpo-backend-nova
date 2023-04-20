using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IEventRepository
    {
        public Task<IEnumerable<Event>> GetAll();
        public Task<Event> Get(int id);
        public Task Add(Event _event);
        public Task Update(Event _event);
    }

    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

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



        public async Task<Event> Get(int id)
        {
            return await _context.Events.Where(_event => _event.Id == id)
                                        .Select(_event => new Event
                                        {
                                            Id = _event.Id,
                                            Name = _event.Name,
                                            Description = _event.Description,
                                            Date = _event.Date,
                                            Start = _event.Start,
                                            End = _event.End,
                                            Location = _event.Location,
                                            Host = _event.Host,
                                            Language = _event.Language,
                                            Capacity = _event.Capacity,
                                            TicketCount = _event.Tickets.Count()
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task Add(Event _event)
        {
            _context.Events.Add(_event);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Event _event)
        {
            _context.Entry(_event).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
