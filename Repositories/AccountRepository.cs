using BankOfHogwarts.Models.Enums;
using BankOfHogwarts.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankOfHogwarts.DTOs;

namespace BankOfHogwarts.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankContext _context;

        public AccountRepository(BankContext context)
        {
            _context = context;
        }
        private async Task UpdateCIBILScore(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);

            // Calculate total inbound and outbound transactions for the account
            var inboundTransactions = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.Credit.HasValue)
                .SumAsync(t => t.Credit.Value);

            var outboundTransactions = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.Debit.HasValue)
                .SumAsync(t => t.Debit.Value);

            // Formula to calculate CIBIL Score based on inbound and outbound transactions
            decimal cibilScore = account.CIBILScore;
            cibilScore += inboundTransactions * 0.0005m;  // Increase by 0.05% of total inbound
            cibilScore -= outboundTransactions * 0.001m;  // Decrease by 0.1% of total outbound

            //CIBIL score remains within valid bounds 
            account.CIBILScore = (int)Math.Clamp(cibilScore, 300, 10000);

            // Updates the account with the new CIBIL score
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public class TransactionLimits
        {
            public const decimal SavingsLimit = 100000;      // 1 Lakh for Savings accounts
            public const decimal SalaryLimit = 300000;       // 3 Lakhs for Salary accounts
            public const decimal BusinessLimit = 100000000;  // 10 Crore for Business accounts
        }


        // Transaction limits for different account types
        private async Task<decimal> GetTotalDailyDebits(int accountId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Calculate total debits (Withdrawals + Transfers) for the current day
            var totalDebits = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.TransactionDate == today)
                .SumAsync(t => t.Debit ?? 0);

            return totalDebits;
        }


        // Withdraw Funds with daily transaction limit check
        public async Task<Transaction> WithdrawFund(int accountId, decimal amount, string pin)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId && a.PIN.ToString() == pin);

            // Check if the account is active and has enough balance

            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive, not approved");
            }
            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Transfer cannot be performed. Account has insufficient balance.");
            }

            // Get total daily debits (withdrawals + transfers) for the current day
            var totalDailyDebits = await GetTotalDailyDebits(accountId);

            // Determine the daily limit based on the account type
            decimal dailyLimit = account.AccountTypeId switch
            {
                1 => TransactionLimits.SavingsLimit,    // Savings
                2 => TransactionLimits.SalaryLimit,     // Salary
                3 => TransactionLimits.BusinessLimit,   // Business
                _ => throw new InvalidOperationException("Unknown account type.")
            };

            // Check if today's transactions (including this one) exceed the daily limit
            if (totalDailyDebits + amount > dailyLimit)
            {
                throw new InvalidOperationException($"Daily transaction limit of {dailyLimit:C} exceeded.");
            }

            // Proceed with withdrawal logic
            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionType = TransactionType.Withdrawal,
                Debit = amount,
                UpdatedBalance = account.Balance - amount,
                Description = "Self Withdrawal",
                TransactionDate = DateOnly.FromDateTime(DateTime.Now)  // Set today's date
            };

            account.Balance -= amount;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await UpdateCIBILScore(accountId);
            return transaction;
        }

        // Transfer Money with daily transaction limit check
        public async Task<Transaction> TransferMoney(int accountId, int beneficiaryId, decimal amount, string pin)
        {
            // Fetch the account details
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId && a.PIN.ToString() == pin);

            // Check if the account exists, is active, and has sufficient balance
            if (account == null)
            {
                throw new InvalidOperationException("Account not found.");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is inactive or not approved.");
            }
            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            // Fetch the beneficiary and check if it belongs to the same account
            var beneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(b => b.BeneficiaryId == beneficiaryId && b.AccountId == accountId);
            if (beneficiary == null)
            {
                throw new InvalidOperationException("Beneficiary not found for this account.");
            }

            // Get total daily debits (withdrawals + transfers) for the current day
            var totalDailyDebits = await GetTotalDailyDebits(accountId);

            // Determine the daily limit based on the account type
            decimal dailyLimit = account.AccountTypeId switch
            {
                1 => TransactionLimits.SavingsLimit,    // Savings
                2 => TransactionLimits.SalaryLimit,     // Salary
                3 => TransactionLimits.BusinessLimit,   // Business
                _ => throw new InvalidOperationException("Unknown account type.")
            };

            // Check if today's transactions (including this one) exceed the daily limit
            if (totalDailyDebits + amount > dailyLimit)
            {
                throw new InvalidOperationException($"Daily transaction limit of {dailyLimit:C} exceeded.");
            }

            // Proceed with transfer logic
            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionType = TransactionType.Transfer,
                Debit = amount,
                UpdatedBalance = account.Balance - amount,
                Description = $"Transfer to Beneficiary Account: {beneficiary.AccountNumber}",
                TransactionDate = DateOnly.FromDateTime(DateTime.Now)  // Set today's date
            };

            account.Balance -= amount;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Update the CIBIL score after the transfer
            await UpdateCIBILScore(accountId);

            return transaction;
        }


        // Display Account Details
        public async Task<Account> DisplayAccountDetails(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            /*if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive, not approved");
            }*/
            return await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Transactions)
                .Include(a => a.Loans)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        // Display Transactions by Date, Month, Week
        public async Task<IEnumerable<Transaction>> DisplayTransactions(int accountId, DateTime? startDate, DateTime? endDate)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");

            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive or not approved");
            }

            // Query to filter transactions based on AccountId, and optionally filter by date range
            var transactionsQuery = _context.Transactions
                .Where(t => t.AccountId == accountId);

            if (startDate.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.TransactionDate >= DateOnly.FromDateTime(startDate.Value));
            }

            if (endDate.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.TransactionDate <= DateOnly.FromDateTime(endDate.Value));
            }

            // Select the required fields and convert enum to string
            var transactionsWithTypes = transactionsQuery
                .Select(t => new Transaction
                {
                    TransactionId = t.TransactionId,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType, // Convert enum to string
                    Description = t.Description,
                    Debit = t.Debit,
                    Credit = t.Credit,
                    UpdatedBalance = t.UpdatedBalance
                });

            return await transactionsWithTypes.ToListAsync();
        }

        // Perform Transactions
        public async Task<Transaction> PerformTransaction(int accountId, Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            // Check if the account exists and is active
            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive, not approved");
            }

            // Proceed with the transaction logic
            switch (transaction.TransactionType)
            {
                case TransactionType.Withdrawal:
                    account.Balance -= transaction.Debit ?? 0;
                    transaction.Description = "Self Withdrawal";
                    break;

                case TransactionType.Deposit:
                    account.Balance += transaction.Credit ?? 0;
                    transaction.Description = "Deposit to Account";
                    break;

                case TransactionType.Transfer:
                    var beneficiary = await _context.Beneficiaries.FindAsync(transaction.AccountId);
                    account.Balance -= transaction.Debit ?? 0;
                    transaction.Description = $"Transfer to Beneficiary Account: {beneficiary?.AccountNumber ?? "Unknown"}";
                    break;

                case TransactionType.Loan:
                    account.Balance += transaction.Credit ?? 0;
                    transaction.Description = "Amount sent by bank (Loan Disbursement)";
                    break;
            }

            transaction.UpdatedBalance = account.Balance;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> DepositFund(int accountId, decimal amount, string pin)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId && a.PIN.ToString() == pin);

            // Check if the account is active
            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive, not approved");
            }


            // Proceed with deposit logic
            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionType = TransactionType.Deposit,
                Credit = amount,
                UpdatedBalance = account.Balance + amount,
                Description = "Deposit to Account"
            };

            account.Balance += amount;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            await UpdateCIBILScore(accountId);

            return transaction;
        }

        // Change PIN
        public async Task<bool> ChangePin(int accountId, string oldPin, string newPin)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId && a.PIN.ToString() == oldPin);
            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive, not approved");
            }

            account.PIN = int.Parse(newPin);
            await _context.SaveChangesAsync();

            return true;
        }

        // Add Beneficiary
        public async Task<Beneficiary> AddBeneficiary(int accountId, Beneficiary beneficiary)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);

            // Check if the account is active
            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive, not approved");
            }



            // Fetch the branch details using the BranchId from the Beneficiary request
            var branch = await _context.Branches.FindAsync(beneficiary.BranchId);
            if (branch == null)
            {
                return null;
            }

            // Set the retrieved branch and account information to the beneficiary object
            beneficiary.Branch = branch;
            beneficiary.Account = account;

            // Add the beneficiary to the database
            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();

            return beneficiary;
        }

        // Loan Management
        public async Task<Loan> ApplyLoan(int accountId, Loan loan)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");
            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive, not approved");
            }

            // Set fields that you don't want the user to provide directly
            loan.LoanApplicationStatus = LoanApplicationStatus.UnderReview;
            loan.Remarks = "Application received";
            loan.LoanStatus = LoanStatus.Pending;
            loan.ApprovedDate = null;
            loan.DisbursementDate = null;
            loan.EmployeeId = null;

            loan.AccountId = accountId;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<IEnumerable<LoanHistoryDto>> LoanHistory(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            // Check if the account exists and is active
            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }
            if (account.Status == AccountStatus.Closed)
                throw new InvalidOperationException("This Account is Closed");

            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("Account is either inactive or not approved");
            }

            // Fetch loans and include related LoanOptions table
            var loansQuery = _context.Loans
                .Where(l => l.AccountId == accountId)
                .Include(l => l.LoanOptions) // Join with LoanOptions table
                .AsQueryable(); // Ensure it remains IQueryable for further filtering

            // Apply date filters if provided

            // Project into a DTO including the LoanOptions properties
            var loans = await loansQuery
                .Select(l => new LoanHistoryDto
                {
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
                    Remarks = l.Remarks
                })
                .ToListAsync();

            return loans;
        }

        /*public async Task<bool> DeleteAccount(int accountId)
        {
            // Find the account along with any related loans
            var account = await _context.Accounts
                .Include(a => a.Loans)  // Include the related Loans
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            // If the account doesn't exist, return false
            if (account == null)
            {
                return false; // Account not found
            }

            // Check if the account has any active loans
            if (account.Loans.Any(l => l.LoanApplicationStatus != LoanApplicationStatus.Rejected))
            {
                throw new InvalidOperationException("Account cannot be deleted because it has loans that are not rejected.");
            }

            // No active loans, proceed to delete the account
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return true; // Account deleted successfully
        }*/

        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesByAccountId(int accountId)
        {
            return await _context.Beneficiaries
                                 .Where(b => b.AccountId == accountId)
                                 .ToListAsync();
        }

        public async Task<bool> RequestDeactivation(int accountId)
        {
            var account = await _context.Accounts
               .Include(a => a.Loans)  // Include the related Loans
               .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                return false;


            // Check if the account is already inactive
            if (account.Status == AccountStatus.Inactive)
                throw new InvalidOperationException("Account is inactive already");

            if (account.Loans.Any(l => l.LoanStatus != LoanStatus.Closed))
            {
                throw new InvalidOperationException("Account cannot be deleted because it has Active loans.");
            }

            // Set account status to Pending for deactivation request
            account.Status = AccountStatus.PendingApproval;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}