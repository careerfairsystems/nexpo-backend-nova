using Nexpo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo.Repositories
{
    public interface IFAQRepository
    {
        public Task<IEnumerable<FrequentAskedQuestion>> GetAll();
        public Task<FrequentAskedQuestion> Get(int id);
        public Task Add(FrequentAskedQuestion question);
        public Task Remove(FrequentAskedQuestion question);
        public Task Update(FrequentAskedQuestion question);
    }


    public class FAQRepository : IFAQRepository

    {
        private readonly ApplicationDbContext _context;

        public FAQRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FrequentAskedQuestion>> GetAll()
        {
            return await _context.FrequentAskedQuestion.OrderBy(q => q.Id).ToListAsync();;
        }

        public async Task<FrequentAskedQuestion> Get(int id)
        {
            return await _context.FrequentAskedQuestion.Where(q => q.Id == id).FirstOrDefaultAsync();
        }

        public async Task Add(FrequentAskedQuestion question)
        {
            _context.FrequentAskedQuestion.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(FrequentAskedQuestion question)
        {
            _context.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task Update(FrequentAskedQuestion question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }

}