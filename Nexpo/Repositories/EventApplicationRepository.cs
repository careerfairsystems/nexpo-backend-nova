using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IEventApplicationRepository
    {
        public Task<bool> ApplicationExists(int studentId, int companyId);
        public Task<EventApplication> GetByEventAndStudent(int studentId, int eventId);
        public Task<IEnumerable<EventApplication>> GetAllForStudent(int studentId);
        public Task<IEnumerable<EventApplication>> GetAllForCompany(int companyId);
        public Task<EventApplication> Get(int id);
        public Task Add(EventApplication application);
        public Task Update(EventApplication application);
        public Task Remove(EventApplication application);
    }

    public class EventApplicationRepository : IEventApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public EventApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApplicationExists(int studentId, int eventId)
        {
            return await _context.EventApplications.AnyAsync(a => a.StudentId == studentId && a.EventId == eventId);
        }

        public async Task<EventApplication> GetByEventAndStudent(int studentId, int eventId)
        {
            return await _context.EventApplications.Where(a => a.StudentId == studentId && a.EventId == eventId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EventApplication>> GetAllForStudent(int studentId)
        {
            return await _context.EventApplications.Where(a => a.StudentId == studentId).ToListAsync();
        }

        public async Task<IEnumerable<EventApplication>> GetAllForCompany(int companyId)
        {
            return await _context.EventApplications.Where(a => a.CompanyId == companyId).ToListAsync();
        }
        
        public async Task<EventApplication> Get(int id)
        {
            return await _context.EventApplications.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(EventApplication application)
        {
            _context.EventApplications.Add(application);
            await _context.SaveChangesAsync();
        }
        
        public async Task Update(EventApplication application)
        {
            _context.Entry(application).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task Remove(EventApplication application)
        {
            _context.EventApplications.Remove(application);
            await _context.SaveChangesAsync();
        }
    }
}
