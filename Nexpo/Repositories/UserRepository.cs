using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IUserRepository
    {
        public Task<bool> UserExists(int id);
        public Task<IEnumerable<User>> GetAll();
        public Task<User> Get(int id);
        public Task<User> FindByEmail(string email);
        public Task Add(User user);
        public Task Remove(User user);
        public Task Update(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync();
        }


        public async Task<User> Get(int id)
        {
            return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public Task<User> FindByEmail(string email)
        {
            return _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
             await _context.SaveChangesAsync();
        }
    }
}
