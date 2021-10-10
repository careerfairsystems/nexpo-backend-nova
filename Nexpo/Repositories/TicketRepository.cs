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
        public Task<Ticket> Get(int id);
        public Task<Ticket> GetByCode(Guid code);
        public Task Add(Ticket ticket);
        public Task Update(Ticket ticket);
        public Task Remove(Ticket ticket);

    }

    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TicketExists(int eventId, int userId)
        {
            return await _context.Tickets.AnyAsync(t => t.EventId == eventId && t.UserId == userId);
        }

        public async Task<IEnumerable<Ticket>> GetAllForUser(int userId)
        {
            return await _context.Tickets.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<Ticket> Get(int id)
        {
            return await _context.Tickets.Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Ticket> GetByCode(Guid code)
        {
            return await _context.Tickets.Where(t => t.Code == code).FirstOrDefaultAsync();
        }

        public async Task Add(Ticket ticket)
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
