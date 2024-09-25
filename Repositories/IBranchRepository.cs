using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;

namespace BankOfHogwarts.Repositories
{
    public interface IBranchRepository
    {
        // Display Branch Details
        Task<IEnumerable<Branch>> DisplayBranchDetails();

        // Display Branch by ID
        Task<Branch> DisplayBranchById(int branchId);
    }
}
