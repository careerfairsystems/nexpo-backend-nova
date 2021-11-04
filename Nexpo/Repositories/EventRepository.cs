using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IEventRepository
    {
        public Task<IEnumerable<Event>> GetAll();
        public Task<Event> Get(int id);
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
            return await _context.Events.Where(e => e.Id == id).Select(e => new Event
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
            }).FirstOrDefaultAsync();
        }
    }
}
