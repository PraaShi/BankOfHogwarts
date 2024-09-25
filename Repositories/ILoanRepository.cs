using BankOfHogwarts.Models;

namespace BankOfHogwarts.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan> GetLoanById(int loanId);  // Fetch loan along with account and customer details
        Task UpdateLoan(Loan loan);          // Update loan details (e.g., after disbursement)
    }

}
