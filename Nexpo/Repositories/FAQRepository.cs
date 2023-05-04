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
        public Task Add(FrequentAskedQuestion question)
        {
            _context.FrequentAskedQuestion.add(question);
        }

        public Task<FrequentAskedQuestion> Get(int id)
        {
            return _context.FrequentAskedQuestion.Where(faq => faq.Id == id).FirstOrDefaultAsync();
        }

        public Task Remove(FrequentAskedQuestion question)
        {
            _context.FrequentAskedQuestion.Remove(question);
        }
        public Task Update(FrequentAskedQuestion question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<FrequentAskedQuestion> GetAll()
        {
            var question = await _context.FrequentAskedQuestion.OrderBy(q => q.Id).ToListAsync();
            return question;
        }
}
}