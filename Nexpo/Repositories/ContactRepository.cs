using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IContactRepository
    {
        public Task<bool> ContactExists(string email);
        public Task<IEnumerable<Contact>> GetAll();
        public Task<Contact> Get(int id);
        public Task Add(Contact contact);
        public Task Remove(Contact contact);
        public Task Update(Contact contact);
    }

    public class ContactRepository :IContactRepository

    {

        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ContactExists(string email)
        {
            return await _context.Contacts.AnyAsync(contact => contact.Email == email);
        }
        

        public async Task<IEnumerable<Contact>> GetAll()
        {
            var contacts = await _context.Contacts.OrderBy(contact => contact.Id).ToListAsync(); 
            return contacts;
        }

        public Task<Contact> Get(int id)
        {
            return _context.Contacts.Where(contact => contact.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Contact contact)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Contact contact)
        {
            _context.Entry(contact).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
