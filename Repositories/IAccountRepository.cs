using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.DTOs;
using BankOfHogwarts.Models;


namespace BankOfHogwarts.Repositories
{
    public interface IAccountRepository
    {
        // Display Account Details
        Task<Account> DisplayAccountDetails(int accountId);

        // Display Transactions by Date, Month, Week
        Task<IEnumerable<Transaction>> DisplayTransactions(int accountId, DateTime? startDate, DateTime? endDate);

        // Perform Transactions
        Task<Transaction> PerformTransaction(int accountId, Transaction transaction);

        Task<Transaction> DepositFund(int accountId, decimal amount, string pin);
        Task<Transaction> WithdrawFund(int accountId, decimal amount, string pin);
        Task<Transaction> TransferMoney(int accountId, int beneficiaryId, decimal amount, string pin);

        // Change PIN
        Task<bool> ChangePin(int accountId, string oldPin, string newPin);

        // Add Beneficiary
        Task<Beneficiary> AddBeneficiary(int accountId, Beneficiary beneficiary);

        // Loan Management
        Task<Loan> ApplyLoan(int accountId, Loan loan);
        Task<IEnumerable<LoanHistoryDto>> LoanHistory(int accountId);

        // Delete Account
        //Task<bool> DeleteAccount(int accountId);
      

        // Method to get all beneficiaries by account ID
        Task<IEnumerable<Beneficiary>> GetBeneficiariesByAccountId(int accountId);

        Task<bool> RequestDeactivation(int accountId,string pin);
    }
}
