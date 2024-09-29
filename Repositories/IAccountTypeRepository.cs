using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;


namespace BankOfHogwarts.Repositories
{
    public interface IAccountTypeRepository
    {
        // Display all Account Types
        Task<IEnumerable<AccountType>> DisplayAccountType();

        Task<string> GetAccountTypeNameById(int accountTypeId);
    }
}