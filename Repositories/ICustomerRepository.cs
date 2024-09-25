using System.Threading.Tasks;
using BankOfHogwarts.Models;

namespace BankOfHogwarts.Repositories
{
    public interface ICustomerRepository
    {
        // Create Customer (Profile Creation)
        public Task<Customer?> CreateCustomer(Customer customer);

        // Validate Customer for Login
        public Task<Customer?> ValidateCustomer(string username, string password);

        // Display all Accounts owned by the customer
        public Task<IEnumerable<Account>> DisplayAccounts(int customerId);

        // Create Account
        public Task<Account> CreateAccount(int customerId, Account account);


    }
}
