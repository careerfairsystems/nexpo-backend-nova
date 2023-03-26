using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetAll();
        public Task<IEnumerable<Company>> GetAllWithTimeslots();
        public Task<Company> Get(int id);
        public Task<Company> GetWithChildren(int id);
        public Task<Company> FindByUser(int userId);
        public Task<Company> FindByName(string name);
        public Task Add(Company company);
        public Task Remove(Company company);
        public Task Update(Company company);
    }

    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _context.Companies.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetAllWithTimeslots()
        {
            return await _context.Companies.Where(c => c.StudentSessionTimeslots.Any()).OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Company> Get(int id)
        {
            return await _context.Companies.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Company> GetWithChildren(int id)
        {
            return await _context.Companies.Include(c => c.Representatives).Include(c => c.StudentSessionTimeslots)
                .Where(c => c.Id == id).OrderBy(c => c.Name).FirstOrDefaultAsync();
        }

        public async Task<Company> FindByUser(int userId)
        {
            return await _context.Companies.Where(c => c.Representatives.Any(u => u.Id == userId)).FirstOrDefaultAsync();
        }
        public async Task<Company> FindByName(string name)
        {
            return await _context.Companies.Where(c => c.Name == name).FirstOrDefaultAsync();
        }
        public async Task Add(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Company company)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Company company)
        {
            _context.Entry(company).State = EntityState.Modified;
             await _context.SaveChangesAsync();
        }
    }
}
