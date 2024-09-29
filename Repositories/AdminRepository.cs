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

        /*public async Task DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                // Handle the case where the employee does not exist
                throw new Exception("Employee not found.");
            }

            // Update the account status from Active to Closed
            if (employee.Status == EmployeeStatus.Active)
            {
                employee.Status = EmployeeStatus.Closed;
                _context.Employees.Update(employee);

                // Save changes to the database
                return await _context.SaveChangesAsync() > 0;
            }

            // Proceed with deletion if the employee exists
            return false;
        }*/

        public async Task<bool> DeactivateAccountAsync(int employeeId)
        {
            // Find the account by the given accountId
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                return false; // Return false if account is not found
            }

            // Update the account status from Active to Closed
            if (employee.Status == EmployeeStatus.Active)
            {
                employee.Status = EmployeeStatus.Closed;
                _context.Employees.Update(employee);

                // Save changes to the database
                return await _context.SaveChangesAsync() > 0;
            }

            // Return false if the account was not active
            return false;
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


        public async Task<bool> DeactivateCustomerAsync(int customerId)
        {
            // Find the customer by the given customerId
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return false; // Return false if customer is not found
            }

            // Update the customer status from Active to Closed
            if (customer.Status == CustomerStatus.Active)
            {
                customer.Status = CustomerStatus.InActive;
                _context.Customers.Update(customer);

                // Save changes to the database
                return await _context.SaveChangesAsync() > 0;
            }

            // Return false if the customer was not active
            return false;
        }

        public async Task<AdminDashboardReport> GenerateAdminDashboardReport()
        {
            try
            {
                // Employee Count
                var employeeCount = await _context.Employees.CountAsync();

                // Customer Count
                var customerCount = await _context.Customers.CountAsync();

                // Male and Female Customer Count
                var maleCustomerCount = await _context.Customers.Where(c => c.Gender == "Male").CountAsync();
                var femaleCustomerCount = await _context.Customers.Where(c => c.Gender == "Female").CountAsync();

                // Total Transactions Count
                var totalTransactions = await _context.Transactions.CountAsync();

                // Total Deposits
                var totalDeposits = await _context.Transactions
                    .Where(t => t.TransactionType == TransactionType.Deposit)
                    .SumAsync(t => t.Credit ?? 0);

                // Total Withdrawals
                var totalWithdrawals = await _context.Transactions
                    .Where(t => t.TransactionType == TransactionType.Withdrawal || t.TransactionType == TransactionType.Transfer)
                    .SumAsync(t => t.Debit ?? 0);

                // Total Loans Disbursed
                var totalLoansDisbursed = await _context.Loans
                    .Where(l => l.LoanStatus == LoanStatus.Disbursed)
                    .Join(_context.LoanOptions,
                        loan => loan.LoanTypeId,
                        loanOption => loanOption.LoanTypeId,
                        (loan, loanOption) => loanOption.LoanAmount)
                    .SumAsync();

                // Interest Income
                var interestIncome = await _context.Loans
                    .Where(l => l.LoanStatus == LoanStatus.Disbursed)
                    .Join(_context.LoanOptions,
                        loan => loan.LoanTypeId,
                        loanOption => loanOption.LoanTypeId,
                        (loan, loanOption) => new { loanOption.LoanAmount, loanOption.InterestRate })
                    .SumAsync(lo => lo.LoanAmount * (lo.InterestRate / 100));

                // Active Accounts Count
                var activeAccountsCount = await _context.Accounts
                    .Where(a => a.Status == AccountStatus.Active)
                    .CountAsync();

                // Average Account Balance
                var averageAccountBalance = await _context.Accounts
                    .Where(a => a.Status == AccountStatus.Active)
                    .AverageAsync(a => a.Balance);

                // Loan Approval Rate
                var totalLoanApplications = await _context.Loans.CountAsync();
                var approvedLoansCount = await _context.Loans
                    .Where(l => l.LoanApplicationStatus == LoanApplicationStatus.Approved)
                    .CountAsync();
                var loanApprovalRate = (totalLoanApplications > 0)
                    ? (approvedLoansCount / (double)totalLoanApplications) * 100 : 0;

                // Top Performing Branch
                var topBranch = await _context.Accounts
                    .GroupBy(a => a.BranchId)
                    .Select(g => new
                    {
                        BranchId = g.Key,
                        TotalBalance = g.Sum(a => a.Balance)
                    })
                    .OrderByDescending(b => b.TotalBalance)
                    .FirstOrDefaultAsync();

                // Top Customers by Total Balance
                var topCustomers = await _context.Accounts
                    .GroupBy(a => a.CustomerId)
                    .Select(g => new TopCustomer
                    {
                        CustomerId = g.Key,
                        TotalBalance = g.Sum(a => a.Balance)
                    })
                    .OrderByDescending(c => c.TotalBalance)
                    .Take(5)
                    .ToListAsync();

                // Overdue Loans Count
                var overdueLoansCount = await _context.Loans
                    .Where(l => l.LoanStatus == LoanStatus.Disbursed &&
                                l.DisbursementDate != null &&
                                l.DisbursementDate.Value.AddMonths(l.LoanOptions.Tenure) < DateTime.Now)
                    .CountAsync();

                // Average Loan Amount
                var averageLoanAmount = await _context.Loans
                    .Where(l => l.LoanStatus == LoanStatus.Disbursed)
                    .Join(_context.LoanOptions,
                        loan => loan.LoanTypeId,
                        loanOption => loanOption.LoanTypeId,
                        (loan, loanOption) => loanOption.LoanAmount)
                    .AverageAsync();

                // Transaction Volume by Type
                var transactionVolumeByType = await _context.Transactions
                    .GroupBy(t => t.TransactionType)
                    .Select(g => new
                    {
                        TransactionType = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(t => t.TransactionType, t => t.Count);

                // Customer Growth (New Customers in the last month)
                var newCustomersCount = await _context.Accounts
                    .Where(c => c.CreatedAt >= DateTime.Now.AddMonths(-1))
                    .CountAsync();

                // Loans to Deposits Ratio
                double loansToDepositsRatio = totalDeposits > 0 ? (double)(totalLoansDisbursed / totalDeposits) * 100 : 0;

                return new AdminDashboardReport
                {
                    EmployeeCount = employeeCount,
                    CustomerCount = customerCount,
                    MaleCustomerCount = maleCustomerCount,
                    FemaleCustomerCount = femaleCustomerCount,
                    TotalTransactions = totalTransactions,
                    TotalDeposits = totalDeposits,
                    TotalWithdrawals = totalWithdrawals,
                    TotalLoansDisbursed = totalLoansDisbursed,
                    InterestIncome = interestIncome,
                    ActiveAccountsCount = activeAccountsCount,
                    AverageAccountBalance = averageAccountBalance,
                    LoanApprovalRate = loanApprovalRate,
                    TopBranch = topBranch?.BranchId.ToString(),
                    TopCustomers = topCustomers,
                    OverdueLoansCount = overdueLoansCount,
                    AverageLoanAmount = averageLoanAmount,
                    TransactionVolumeByType = transactionVolumeByType,
                    NewCustomersCount = newCustomersCount,
                    LoansToDepositsRatio = loansToDepositsRatio
                };
            }
            catch (DivideByZeroException ex)
            {
                // Handle divide by zero errors
                Console.WriteLine("Error: Division by zero in report calculation.");
                throw new Exception("Division by zero occurred in report calculations.", ex);
            }
            catch (InvalidOperationException ex)
            {
                // Handle cases like data not found
                Console.WriteLine("Error: Data not found or invalid operation.");
                throw new Exception("Invalid operation occurred while generating the report.", ex);
            }
            catch (Exception ex)
            {
                // Generic error handling
                Console.WriteLine("An error occurred while generating the admin dashboard report.");
                throw new Exception("An error occurred while generating the admin dashboard report.", ex);
            }
        }

    }
}
