using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankOfHogwarts.Models;

namespace BankOfHogwarts.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly BankContext _context;

        public BranchRepository(BankContext context)
        {
            _context = context;
        }

        // Display Branch Details
        public async Task<IEnumerable<Branch>> DisplayBranchDetails()
        {
            return await _context.Branches.ToListAsync();
        }

        // Display Branch by ID
        public async Task<Branch?> DisplayBranchById(int branchId)
        {
            return await _context.Branches.FindAsync(branchId);
        }
    }
}
