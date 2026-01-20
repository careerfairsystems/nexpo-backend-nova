using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IStudentSessionApplicationRepository
    {
        public Task<bool> ApplicationExists(int applierId, int companyId);
        public Task<StudentSessionApplication> GetByCompanyAndStudent(int applierId, int companyId);
        public Task<IEnumerable<StudentSessionApplication>> GetAllForApplier(int applierId);
        public Task<IEnumerable<StudentSessionApplication>> GetAllForCompany(int companyId);
        public Task<StudentSessionApplication> Get(int id);
        public Task Add(StudentSessionApplication application);
        public Task Update(StudentSessionApplication application);
        public Task Remove(StudentSessionApplication application);
    }

    public class StudentSessionApplicationRepository : IStudentSessionApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentSessionApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApplicationExists(int applierId, int companyId)
        {
            return await _context.StudentSessionApplications.AnyAsync(a => a.StudentId == applierId && a.CompanyId == companyId);
        }

        public async Task<StudentSessionApplication> GetByCompanyAndStudent(int applierId, int companyId)
        {
            return await _context.StudentSessionApplications.Where(a => a.StudentId == applierId && a.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StudentSessionApplication>> GetAllForApplier(int applierId)
        {
            return await _context.StudentSessionApplications.Where(a => a.StudentId == applierId).ToListAsync();
        }

        public async Task<IEnumerable<StudentSessionApplication>> GetAllForCompany(int companyId)
        {
            return await _context.StudentSessionApplications.Where(a => a.CompanyId == companyId).ToListAsync();
        }
        
        public async Task<StudentSessionApplication> Get(int id)
        {
            return await _context.StudentSessionApplications.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(StudentSessionApplication application)
        {
            _context.StudentSessionApplications.Add(application);
            await _context.SaveChangesAsync();
        }
        
        public async Task Update(StudentSessionApplication application)
        {
            _context.Entry(application).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task Remove(StudentSessionApplication application)
        {
            _context.StudentSessionApplications.Remove(application);
            await _context.SaveChangesAsync();
        }
    }
}
