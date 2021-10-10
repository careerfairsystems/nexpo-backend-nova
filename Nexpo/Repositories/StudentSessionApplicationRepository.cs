using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IStudentSessionApplicationRepository
    {
        public Task<bool> ApplicationExists(int studentId, int companyId);
        public Task<IEnumerable<StudentSessionApplication>> GetAllUnassigned();
        public Task<IEnumerable<StudentSessionApplication>> GetAllForStudent(int studentId);
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

        public async Task<bool> ApplicationExists(int studentId, int companyId)
        {
            return await _context.StudentSessionApplications.AnyAsync(a => a.StudentId == studentId && a.CompanyId == companyId);
        }

        public async Task<IEnumerable<StudentSessionApplication>> GetAllForStudent(int studentId)
        {
            return await _context.StudentSessionApplications.Where(a => a.StudentId == studentId).ToListAsync();
        }

        public async Task<IEnumerable<StudentSessionApplication>> GetAllForCompany(int companyId)
        {
            return await _context.StudentSessionApplications.Where(a => a.CompanyId == companyId).ToListAsync();
        }

        public async Task<IEnumerable<StudentSessionApplication>> GetAllUnassigned()
        {
            return await _context.StudentSessionApplications.Where(a => a.StudentSession == null).ToListAsync();
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
