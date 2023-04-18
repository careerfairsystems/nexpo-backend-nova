using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IContactReposity
    {
        public Task<bool> ContactExists(int id);
        public Task<IEnumerable<Contact>> GetAll();
        public Task<Contact> Get(int id);
        public Task Add(Contact contact);
        public Task Remove(Contact contact);
        public Task Update(Contact contact);
    }

    public class ContactReposity : IContactReposity
    {

        private readonly ApplicationDbContext _context;

        public async Task<bool> ContactExists(int id)
        {
            return await _context.Contacts.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetAll()
        {
            return await _context.Contacts.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync();
        }

        public Task<Contact> Get(int id)
        {
            return _context.Contacts.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        public Task Remove(Contact contact)
        {
            _context.Contacts.Remove(contact);
            return _context.SaveChangesAsync();
        }

        public async Task Update(Contact contact)
        {
            _context.Entry(contact).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }



    }
}
