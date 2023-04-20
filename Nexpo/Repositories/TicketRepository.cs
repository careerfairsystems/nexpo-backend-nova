using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface ITicketRepository
    {
        public Task<bool> TicketExists(int eventId, int userId);
        public Task<IEnumerable<Ticket>> GetAllForUser(int userId);
        public Task<IEnumerable<Ticket>> GetAllForEvent(int eventId);
        public Task<Ticket> Get(int id);
        public Task<Ticket> GetByCode(Guid code);
        public Task<bool> Add(Ticket ticket);
        public Task AddAdmin(Ticket ticket);
        public Task Update(Ticket ticket);
        public Task Remove(Ticket ticket);
        public Task<EventType> GetEventType(int ticketId);

    }

    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventRepository _eventRepo;

        public TicketRepository(ApplicationDbContext context, IEventRepository iEventRepo)
        {
            _context = context;
            _eventRepo = iEventRepo;
        }

        public async Task<bool> TicketExists(int eventId, int userId)
        {
            return await _context.Tickets.AnyAsync(t => t.EventId == eventId && t.UserId == userId);
        }

        public async Task<IEnumerable<Ticket>> GetAllForUser(int userId)
        {
            return await _context.Tickets.Include(t => t.Event).Where(t => t.UserId == userId)
                .OrderBy(t => t.Event.Date).ThenBy(t => t.Event.Start).ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllForEvent(int eventId)
        {
            return await _context.Tickets.Where(t => t.EventId == eventId)
                .OrderBy(t => t.User.FirstName).ThenBy(t => t.User.LastName).ToListAsync();
        }

        public async Task<EventType> GetEventType(int ticketId)
        {
            return await _context.Tickets.Include(t => t.Event).Where(t => t.Id == ticketId).Select(t => t.Event.Type).FirstOrDefaultAsync();

        }

        public async Task<Ticket> Get(int id)
        {
            return await _context.Tickets.Include(t => t.Event).Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Ticket> GetByCode(Guid code)
        {
            return await _context.Tickets.Include(t => t.Event).Where(t => t.Code == code).FirstOrDefaultAsync();
        }

        public async Task<bool> Add(Ticket ticket)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                var _event = await _eventRepo.Get(ticket.EventId);
                
                // Don't add ticket if limit is reached
                if (_event.TicketCount >= _event.Capacity) {
                    return false;
                }
                
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                dbContextTransaction.Commit();
                return true;
            }
        }

        public async Task AddAdmin(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Ticket ticket)
        {
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
