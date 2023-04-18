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
        public Task<List<Contact>> GetAll();
        public Task<Contact> Get(int id);
        public Task Add(Contact contact);
        public Task Remove(Contact contact);
        public Task Update(Contact contact);
    }

    public class ContactReposity : IContactReposity
    {

        private readonly ApplicationDbContext _context;

        public ContactReposity(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ContactExists(int id)
        {
            return await _context.Contacts.AnyAsync(contact => contact.Id == id);
        }

        public async Task<List<Contact>> GetAll()
        {
            
            var contacts = await _context.Contacts.OrderBy(contact => contact.FirstName).ThenBy(contact => contact.LastName).ToListAsync();
            System.Console.WriteLine(contacts);
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
