using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;
using static BankOfHogwarts.Repositories.EmployeeRepository;


namespace BankOfHogwarts.Repositories
{
    public interface IEmployeeRepository
    {
        // Manage Account Requests (Approve/Reject)
        Task<bool> ManageAccountRequest(int accountId, bool isApproved);

        // Manage Loan Requests (Approve/Reject based on credit score)
        Task<bool> ManageLoanRequest(int loanId, bool isApproved, int employeeId);

        // Disburse Loan Amount to Customer's Account
        Task<bool> DisburseLoanToAccount(int loanId, bool isApproved);

        // Display Transactions by Account ID (between dates, month, week, etc.)
        Task<IEnumerable<Transaction>> DisplayTransactionsByAccountID(int accountId, DateTime? startDate, DateTime? endDate);

        // Manage Account Deletion Request (Approve/Reject)
        Task<bool> ManageAccountDeletionRequest(int accountId);

        Task<FinancialReport> GenerateFinancialReports();

        Task<bool> CloseLoans(int loanId);

        Task<bool> DeactivateAccount(int accountId);
    }
}
