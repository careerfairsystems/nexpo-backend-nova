using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IStudentRepository
    {
        public Task<Student> Get(int id);
        public Task<Student> FindByUser(int userId);
        public Task Add(Student user);
        public Task Update(Student user);
        public Task Remove(Student user);
    }

    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student> FindByUser(int userId)
        {
            return await _context.Students.Where(s => s.User.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<Student> Get(int id)
        {
            return await _context.Students.Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Student student)
        {
            _context.Entry(student).State = EntityState.Modified;
             await _context.SaveChangesAsync();
        }

        public async Task Remove(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
