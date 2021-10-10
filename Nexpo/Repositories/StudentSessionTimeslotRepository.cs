using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IStudentSessionTimeslotRepository
    {
        public Task<IEnumerable<StudentSessionTimeslot>> GetAllUnassigned();
        public Task<IEnumerable<StudentSessionTimeslot>> GetAllForCompany(int companyId);
        public Task<StudentSessionTimeslot> Get(int id);
        public Task Add(StudentSessionTimeslot timeslot);
        public Task Update(StudentSessionTimeslot timeslot);
        public Task Remove(StudentSessionTimeslot timeslot);
    }

    public class StudentSessionTimeslotRepository : IStudentSessionTimeslotRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentSessionTimeslotRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentSessionTimeslot>> GetAllUnassigned()
        {
            return await _context.StudentSessionTimeslots.Where(t => t.StudentSession == null).ToListAsync();
        }

        public async Task<IEnumerable<StudentSessionTimeslot>> GetAllForCompany(int companyId)
        {
            return await _context.StudentSessionTimeslots.Where(t => t.CompanyId == companyId).ToListAsync();
        }

        public async Task<StudentSessionTimeslot> Get(int id)
        {
            return await _context.StudentSessionTimeslots.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(StudentSessionTimeslot timeslot)
        {
            _context.StudentSessionTimeslots.Add(timeslot);
            await _context.SaveChangesAsync();
        }
        
        public async Task Update(StudentSessionTimeslot timeslot)
        {
            _context.Entry(timeslot).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task Remove(StudentSessionTimeslot timeslot)
        {
            _context.StudentSessionTimeslots.Remove(timeslot);
            await _context.SaveChangesAsync();
        }
    }
}
