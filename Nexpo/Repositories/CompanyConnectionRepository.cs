using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface ICompanyConnectionRepository
    {
        public Task<bool> ConnectionExists(int studentId, int companyId);
        public Task<IEnumerable<CompanyConnection>> GetAllForStudent(int studentId);
        public Task<IEnumerable<CompanyConnection>> GetAllForCompany(int companyId);
        public Task<CompanyConnection> Get(int id);
        public Task Add(CompanyConnection connection);
        public Task Update(CompanyConnection connection);
        public Task Remove(CompanyConnection connection);

    }

    public class CompanyConnectionRepository : ICompanyConnectionRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyConnectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ConnectionExists(int studentId, int companyId)
        {
            return await _context.CompanyConnections.AnyAsync(c => c.StudentId == studentId && c.CompanyId == companyId);
        }

        public async Task<IEnumerable<CompanyConnection>> GetAllForStudent(int studentId)
        {
            return await _context.CompanyConnections.Where(c => c.StudentId == studentId).ToListAsync();
        }
        public async Task<IEnumerable<CompanyConnection>> GetAllForCompany(int companyId)
        {
            return await _context.CompanyConnections.Where(c => c.CompanyId == companyId).ToListAsync();
        }

        public async Task<CompanyConnection> Get(int id)
        {
            return await _context.CompanyConnections.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(CompanyConnection connection)
        {
            _context.CompanyConnections.Add(connection);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CompanyConnection connection)
        {
            _context.Entry(connection).State = EntityState.Modified;
             await _context.SaveChangesAsync();
        }

        public async Task Remove(CompanyConnection connection)
        {
            _context.CompanyConnections.Remove(connection);
            await _context.SaveChangesAsync();
        }
    }
}
