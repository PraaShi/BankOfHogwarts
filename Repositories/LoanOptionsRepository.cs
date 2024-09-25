using BankOfHogwarts.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankOfHogwarts.Repositories
{
    public class LoanOptionsRepository : ILoanOptionsRepository
    {
        private readonly BankContext _context;

        public LoanOptionsRepository(BankContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoanOptions>> GetAllLoanOptionsAsync()
        {
            return await _context.LoanOptions.ToListAsync();
        }

        public async Task<LoanOptions> GetLoanOptionByIdAsync(int id)
        {
            return await _context.LoanOptions.FindAsync(id);
        }
    }
}
