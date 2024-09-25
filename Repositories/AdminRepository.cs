using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankOfHogwarts.Models;
using BankOfHogwarts.Models.Enums;

namespace BankOfHogwarts.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly BankContext _context;

        public AdminRepository(BankContext context)
        {
            _context = context;
        }

        // Manage Employees
        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                // Handle the case where the employee does not exist
                throw new Exception("Employee not found.");
            }

            // Proceed with deletion if the employee exists
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }


        // Manage Customers
        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerById(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer> AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<List<Loan>> GetLoansByCustomerId(int customerId)
        {
            var loans = await (from loan in _context.Loans
                               join account in _context.Accounts
                               on loan.AccountId equals account.AccountId
                               where account.CustomerId == customerId
                               select loan).ToListAsync();

            return loans;
        }


        public async Task DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                throw new Exception("Customer not found.");
            }

            // Check if the customer has any active loans
            var loans = await GetLoansByCustomerId(id);
            bool hasActiveLoan = loans.Any(l => l.LoanStatus == LoanStatus.Disbursed);

            if (hasActiveLoan)
            {
                throw new Exception("Customer cannot be deleted because they have active loans.");
            }

            // Proceed with deletion if no active loans
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

    }
}
