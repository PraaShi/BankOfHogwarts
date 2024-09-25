using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankOfHogwarts.Models;

namespace BankOfHogwarts.Repositories
{
    public class AccountTypeRepository : IAccountTypeRepository
    {
        private readonly BankContext _context;

        public AccountTypeRepository(BankContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountType>> DisplayAccountType()
        {
            try
            {
                return await _context.AccountTypes.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving account types.", ex);
            }

        }
    }
}
