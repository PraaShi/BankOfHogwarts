using BankOfHogwarts.Controllers;
using BankOfHogwarts.DTOs;
using BankOfHogwarts.Models;
using BankOfHogwarts.Models.Enums;
using log4net;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankOfHogwarts.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly BankContext _context;

        private static readonly ILog log = LogManager.GetLogger(typeof(AccountController));
        public EmployeeRepository(BankContext context)
        {
            _context = context;
        }

        public async Task<bool> ManageAccountRequest(int accountId, bool isApproved)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
                return false;
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            // Update the account status based on the approval
            account.Status = isApproved ? AccountStatus.Active : AccountStatus.Inactive;  // Approve or reject the account

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LoanHistoryDto>> GetAllLoans()
        {
            // Fetch all loans and include related LoanOptions table
            var loansQuery = _context.Loans
                .Include(l => l.LoanOptions) // Join with LoanOptions table
                .AsQueryable(); // Ensure it remains IQueryable for further filtering

            // Project into a DTO including the LoanOptions properties
            var loans = await loansQuery
                .Select(l => new LoanHistoryDto
                {
                    AccountId = l.AccountId,
                    LoanId = l.LoanId,
                    LoanType = l.LoanOptions.LoanType.ToString(), // Convert LoanType enum to string
                    LoanAmount = l.LoanOptions.LoanAmount,
                    InterestRate = l.LoanOptions.InterestRate,
                    Tenure = l.LoanOptions.Tenure,
                    Purpose = l.Purpose,
                    ApplicationDate = l.ApplicationDate,
                    LoanApplicationStatus = l.LoanApplicationStatus.ToString(),
                    ApprovedDate = l.ApprovedDate,
                    LoanStatus = l.LoanStatus.ToString(), // Convert enums to string
                    DisbursementDate = l.DisbursementDate,
                    LoanFinalStatus = l.LoanFinalStatus,
                    ClosedDate = l.ClosedDate,
                    Remarks = l.Remarks
                })
                .ToListAsync();

            return loans;
        }


        public async Task<bool> ManageLoanRequest(int loanId, bool isApproved, int employeeId)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == loanId);
            if (loan == null)
                return false;

            // Check if the loan is in the correct state to be approved/rejected
            if (loan.LoanApplicationStatus != LoanApplicationStatus.UnderReview)
                return false;

            // If approved, update to Approved status and set ApprovedDate and EmployeeId
            if (isApproved)
            {
                loan.LoanApplicationStatus = LoanApplicationStatus.Approved;
                //loan.LoanStatus = LoanStatus.Approved;
                loan.ApprovedDate = DateTime.Now;
                loan.EmployeeId = employeeId; // Record the employee who approved the loan
            }
            else
            {
                // If not approved, mark it as Rejected and Closed
                loan.LoanApplicationStatus = LoanApplicationStatus.Rejected;
                loan.LoanStatus = LoanStatus.Rejected;
                loan.LoanFinalStatus = LoanFinalStatus.Closed;
            }

            // Update the loan in the database
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DisburseLoanToAccount(int loanId, bool isApproved)
        {
            // Fetch the loan along with its related account and loan options
            var loan = await _context.Loans
                                     .Include(l => l.Account)    // Include related account
                                     .Include(l => l.LoanOptions) // Include loan options to get loan amount
                                     .FirstOrDefaultAsync(l => l.LoanId == loanId);

            // Validate loan existence
            if (loan == null)
            {
                Console.WriteLine($"Loan with id {loanId} not found.");
                return false;
            }

            // Fetch the associated account using the loan's AccountId
            var account = loan.Account;

            // Validate account existence
            if (account == null)
            {
                Console.WriteLine($"Account associated with loan {loanId} not found.");
                return false;
            }

            // Ensure the loan is in an Approved state (not UnderReview)
            if (loan.LoanApplicationStatus != LoanApplicationStatus.Approved)
            {
                Console.WriteLine($"Loan {loanId} is not approved. Current status: {loan.LoanApplicationStatus}");
                return false;
            }

            // If the loan is approved, update statuses and disburse the loan amount to the account
            if (isApproved)
            {
                // Update loan status to Disbursed
                loan.LoanStatus = LoanStatus.Disbursed;
                loan.LoanFinalStatus = LoanFinalStatus.Active;
                loan.DisbursementDate = DateTime.Now;

                // Update account balance by adding the loan amount from LoanOptions
                account.Balance += loan.LoanOptions.LoanAmount;

                // Create a transaction for the loan disbursement
                var transaction = new Transaction
                {
                    AccountId = account.AccountId,  // Use account id from loan's related account
                    TransactionType = TransactionType.Loan,
                    Credit = loan.LoanOptions.LoanAmount,
                    UpdatedBalance = account.Balance,
                    Description = "Loan disbursed",
                    TransactionDate = DateOnly.FromDateTime(DateTime.Now)  // Set today's date
                };

                // Add the transaction to the database
                _context.Transactions.Add(transaction);

                // Update loan disbursement date
                loan.DisbursementDate = DateTime.Now;
            }
            else
            {
                // If not approved, mark the loan as Rejected and Closed
                Console.WriteLine($"Rejecting loan {loanId}.");
                loan.LoanApplicationStatus = LoanApplicationStatus.Rejected;
                loan.LoanStatus = LoanStatus.Rejected;
                loan.LoanFinalStatus = LoanFinalStatus.Closed;
            }

            // Save the changes in the database
            _context.Loans.Update(loan);
            _context.Accounts.Update(account); // Only update if loan is approved
            await _context.SaveChangesAsync();

            Console.WriteLine($"Loan {loanId} processed successfully.");
            return true;
        }





        public async Task<IEnumerable<Transaction>> DisplayTransactionsByAccountID(int accountId, DateTime? startDate, DateTime? endDate)
        {
            var transactions = _context.Transactions.AsQueryable();

            transactions = transactions.Where(t => t.AccountId == accountId);

            if (startDate.HasValue)
            {
                var start = DateOnly.FromDateTime(startDate.Value);
                transactions = transactions.Where(t => t.TransactionDate >= start);
            }

            if (endDate.HasValue)
            {
                var end = DateOnly.FromDateTime(endDate.Value);
                transactions = transactions.Where(t => t.TransactionDate <= end);
            }

            return await transactions.ToListAsync();
        }

        /*public async Task<bool> ManageAccountDeletionRequest(int accountId)
        {
            // Find the account along with any related loans
            var account = await _context.Accounts
                .Include(a => a.Loans)  // Include the related loans
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            // If the account doesn't exist, return false
            if (account == null)
                return false;

            // Check if any loans have a status other than Rejected
            if (account.Loans.Any(l => l.LoanApplicationStatus != LoanApplicationStatus.Rejected))
            {
                throw new InvalidOperationException("Account cannot be deleted because it has loans that are not rejected.");
            }

            // Proceed to delete the account if all loans are rejected
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return true; // Account deleted successfully
        }*/
        public class FinancialReport
        {
            public decimal TotalDeposits { get; set; }
            public decimal TotalWithdrawals { get; set; }
            public decimal TotalLoansDisbursed { get; set; }
            public decimal NetAccountBalance { get; set; }
            public decimal InterestIncome { get; set; }
            public int TotalAccounts { get; set; }
            public int TotalLoans { get; set; }
        }


        public async Task<FinancialReport> GenerateFinancialReports()
        {
            // Total Deposits (sum of all credits in the transactions table)
            var totalDeposits = await _context.Transactions
                .Where(t => t.TransactionType == TransactionType.Deposit)
                .SumAsync(t => t.Credit ?? 0);

            // Total Withdrawals (sum of all debits in the transactions table)
            var totalWithdrawals = await _context.Transactions
                .Where(t => t.TransactionType == TransactionType.Withdrawal || t.TransactionType == TransactionType.Transfer)
                .SumAsync(t => t.Debit ?? 0);


            // Query to get the total disbursed loan amount by joining Loan and LoanOptions tables
            var totalDisbursedLoanAmount = await _context.Loans
                .Where(l => l.LoanStatus == LoanStatus.Disbursed)  // Filter loans by disbursed status
                .Join(_context.LoanOptions,  // Join with LoanOptions table
                    loan => loan.LoanTypeId,  // Foreign key in Loan
                    loanOption => loanOption.LoanTypeId,  // Primary key in LoanOptions
                    (loan, loanOption) => new
                    {
                        LoanAmount = loanOption.LoanAmount  // Access LoanAmount from LoanOptions
                    })
                .SumAsync(lo => lo.LoanAmount);  // Sum the LoanAmount



            // Total Loan Repayments (sum of all loan repayments)
            var totalLoanRepayments = await _context.Transactions
                .Where(t => t.TransactionType == TransactionType.Loan && t.Credit.HasValue)
                .SumAsync(t => t.Credit ?? 0);

            // Net Account Balance (sum of balances from all active accounts)
            var netAccountBalance = await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active)
                .SumAsync(a => a.Balance);

            // Interest Income (sum of interest applied to loans)
            var interestIncome = await _context.Loans
                  .Where(l => l.LoanStatus == LoanStatus.Disbursed)  // Filter by disbursed loans
                  .Join(_context.LoanOptions,  // Join with LoanOptions table
                      loan => loan.LoanTypeId,  // Foreign key in Loan
                      loanOption => loanOption.LoanTypeId,  // Primary key in LoanOptions
                      (loan, loanOption) => new
                      {
                          LoanAmount = loanOption.LoanAmount,  // Loan amount from LoanOptions
                          InterestRate = loanOption.InterestRate  // Interest rate from LoanOptions
                      })
                  .SumAsync(lo => lo.LoanAmount * (lo.InterestRate / 100));  // Calculate total interest income

            // Total number of active accounts
            var totalAccounts = await _context.Accounts
                .Where(a => a.Status == AccountStatus.Active)
                .CountAsync();

            // Total number of active loans
            var totalLoans = await _context.Loans
                .Where(l => l.LoanStatus == LoanStatus.Disbursed)
                .CountAsync();

            // Generate the report
            return new FinancialReport
            {
                TotalDeposits = totalDeposits,
                TotalWithdrawals = totalWithdrawals,
                TotalLoansDisbursed = totalDisbursedLoanAmount,
                NetAccountBalance = netAccountBalance,
                InterestIncome = interestIncome,
                TotalAccounts = totalAccounts,
                TotalLoans = totalLoans
            };
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.AccountType)
                .Include(a => a.Branch)
                .Select(a => new Account
                {
                    AccountId = a.AccountId,
                    CustomerId = a.CustomerId,
                    Customer = a.Customer,
                    AccountTypeId = a.AccountTypeId,
                    AccountType = a.AccountType,
                    BranchId = a.BranchId,
                    Branch = a.Branch,
                    AccountNumber = a.AccountNumber,
                    Balance = a.Balance,
                    CIBILScore = a.CIBILScore,
                    CreatedAt = a.CreatedAt,
                    Status = a.Status
                }).ToListAsync();
        }
        public async Task<IEnumerable<Account>> GetAccountsByStatusAsync(AccountStatus? status)
        {
            var query = _context.Accounts.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            return await query.ToListAsync();
        }


        public async Task<bool> DeactivateAccount(int accountId, bool isApproved)
        {
            var account = await _context.Accounts
                .Include(a => a.Loans)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
            {
                Console.WriteLine("Account not found");
                return false;
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("Account is closed already.");

            // Check if any loans are active (based on your business logic, for example)
            if (account.Loans.Any(l => l.LoanFinalStatus != LoanFinalStatus.Closed))
            {
                throw new InvalidOperationException("Account cannot be deleted because it has Active loans.");
            }
            if (isApproved)
            {
                if (account.Status == AccountStatus.OnHold)
                {
                    account.Status = AccountStatus.Closed;
                    Console.WriteLine($"Accepting Deactivation Request {accountId}.");
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // If not approved, mark the loan as Rejected and Closed
                Console.WriteLine($"Rejecting Deactivation Request {accountId}.");
                account.Status = AccountStatus.Active;
            }
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CloseLoans(int loanId)
        {
            // Find the loan by its ID
            var loan = await _context.Loans
                                     .Include(l => l.Account)    
                                     .Include(l => l.LoanOptions) 
                                     .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null)
                return false; // Loan not found

            loan.LoanFinalStatus = LoanFinalStatus.Closed;
            loan.ClosedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
    }

}