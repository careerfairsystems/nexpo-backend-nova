using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IVolunteerRepository
    {
        Task<Volunteer> Get(int id);
        Task<Volunteer> FindByUser(int userId);
        Task Add(Volunteer volunteer);
        Task Update(Volunteer volunteer);
        Task Remove(Volunteer volunteer);
    }

    public class VolunteerRepository : IVolunteerRepository
    {
        private readonly ApplicationDbContext _context;

        public VolunteerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Volunteer> FindByUser(int userId)
        {
            return await _context.Volunteers.Where(v => v.User.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<Volunteer> Get(int id)
        {
            return await _context.Volunteers.Where(volunteer => volunteer.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(Volunteer volunteer)
        {
            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Volunteer volunteer)
        {
            _context.Entry(volunteer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Volunteer volunteer)
        {
            _context.Volunteers.Remove(volunteer);
            await _context.SaveChangesAsync();
        }
    }
}
