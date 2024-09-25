// Interfaces/ILoanOptionsRepository.cs
using BankOfHogwarts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankOfHogwarts.Repositories
{
    public interface ILoanOptionsRepository
    {
        Task<IEnumerable<LoanOptions>> GetAllLoanOptionsAsync();
        Task<LoanOptions> GetLoanOptionByIdAsync(int id);
    }
}