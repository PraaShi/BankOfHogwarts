using BankOfHogwarts.Models;
using BankOfHogwarts.Models.Enums;
using BankOfHogwarts.Repositories;
using Microsoft.EntityFrameworkCore;

public class CustomerRepository : ICustomerRepository
{
    private readonly BankContext _context;

    public CustomerRepository(BankContext context)
    {
        _context = context;
    }

    public async Task<Customer?> CreateCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> ValidateCustomer(string email, string password)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
    }

    public async Task<IEnumerable<Account>> DisplayAccounts(int customerId)
    {
        return await _context.Accounts
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<Account> CreateAccount(int customerId, Account account)
    {
        account.CustomerId = customerId;
        account.AccountNumber = GenerateAccountNumber();  // Auto-generate account number
        account.CIBILScore = 0;  // Set CIBIL score to 0 by default
        account.Status = AccountStatus.PendingApproval;
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public string GenerateAccountNumber()
    {
        // Generate a random 10-digit number for account
        Random random = new Random();
        return random.Next(0, 1000000000).ToString("D10");
    }

  
}
