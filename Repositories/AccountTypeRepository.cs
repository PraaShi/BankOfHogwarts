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

        public async Task<string> GetAccountTypeNameById(int accountTypeId)
        {
            try
            {
                // Find the account type by ID
                var accountType = await _context.AccountTypes
                    .FirstOrDefaultAsync(at => at.AccountTypeId == accountTypeId);

                // Return the name if found, otherwise return null
                return accountType?.AccountTypeName;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the account type name for ID {accountTypeId}.", ex);
            }
        }
    }
}
