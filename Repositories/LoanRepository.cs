using BankOfHogwarts.Models;
using Microsoft.EntityFrameworkCore;

namespace BankOfHogwarts.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly BankContext _context;  

        public LoanRepository(BankContext context)
        {
            _context = context;
        }

        // Method to get loan by its ID, including related account and customer
        public async Task<Loan> GetLoanById(int loanId)
        {
            return await _context.Loans
                .Include(l => l.Account)        
                .ThenInclude(a => a.Customer)   
                .FirstOrDefaultAsync(l => l.LoanId == loanId);  
        }

        // Method to update loan after disbursement
        public async Task UpdateLoan(Loan loan)
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync(); 
        }
    }

}
