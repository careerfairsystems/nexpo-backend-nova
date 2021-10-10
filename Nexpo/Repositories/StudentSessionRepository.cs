using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IStudentSessionRepository
    {
        public Task<IEnumerable<StudentSession>> GetAllForStudent(int studentId);
        public Task<StudentSession> Get(int id);
        public Task Add(StudentSession session);
        public Task Update(StudentSession session);
        public Task Remove(StudentSession session);
    }

    public class StudentSessionRepository : IStudentSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentSession>> GetAllForStudent(int studentId)
        {
            return await _context.StudentSessions.Where(s => s.StudentId == studentId).ToListAsync();
        }

        public async Task<StudentSession> Get(int id)
        {
            return await _context.StudentSessions.Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(StudentSession session)
        {
            _context.StudentSessions.Add(session);
            await _context.SaveChangesAsync();
        }
        
        public async Task Update(StudentSession session)
        {
            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task Remove(StudentSession session)
        {
            _context.StudentSessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }
}
