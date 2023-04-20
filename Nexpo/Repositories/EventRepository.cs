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
        public Task Add(Event even);
        public Task Update(Event even);
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
            return await _context.Events.OrderBy(e => e.Date).ThenBy(e => e.Start).Select(e => new Event
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Date = e.Date,
                Start = e.Start,
                End = e.End,
                Location = e.Location,
                Host = e.Host,
                Language = e.Language,
                Capacity = e.Capacity,
                TicketCount = e.Tickets.Count()
            }).ToListAsync();
        }

        

        public async Task<Event> Get(int id)
        {
            return await _context.Events.Where(_event => _event.Id == id)
                                        .Select(_event => new Event
                                        {
                                            Id = _event.Id,
                                            Name = _event.Name,
                                            Type = _event.Type,
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

        public async Task Add(Event even)
        {
            _context.Events.Add(even);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Event even)
        {
            _context.Entry(even).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
