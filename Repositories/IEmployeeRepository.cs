using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.DTOs;
using BankOfHogwarts.Models;
using BankOfHogwarts.Models.Enums;
using static BankOfHogwarts.Repositories.EmployeeRepository;


namespace BankOfHogwarts.Repositories
{
    public interface IEmployeeRepository
    {
        // Manage Account Requests (Approve/Reject)
        Task<bool> ManageAccountRequest(int accountId, bool isApproved);

        Task<IEnumerable<LoanHistoryDto>> GetAllLoans();

        // Manage Loan Requests (Approve/Reject based on credit score)
        Task<bool> ManageLoanRequest(int loanId, bool isApproved, int employeeId);

        // Disburse Loan Amount to Customer's Account
        Task<bool> DisburseLoanToAccount(int loanId, bool isApproved);
        Task<bool> CloseLoans(int loanId);

        // Display Transactions by Account ID (between dates, month, week, etc.)
        Task<IEnumerable<Transaction>> DisplayTransactionsByAccountID(int accountId, DateTime? startDate, DateTime? endDate);

        // Manage Account Deletion Request (Approve/Reject)
        //Task<bool> ManageAccountDeletionRequest(int accountId);


        Task<FinancialReport> GenerateFinancialReports();
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<IEnumerable<Account>> GetAccountsByStatusAsync(AccountStatus? status);



        Task<bool> DeactivateAccount(int accountId, bool isApproved);
    }
}
