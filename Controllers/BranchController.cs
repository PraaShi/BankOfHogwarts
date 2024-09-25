using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;
using BankOfHogwarts.Repositories;
using Microsoft.AspNetCore.Authorization;
using log4net;

namespace BankOfHogwarts.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;

        // Define the logger
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchController));

        public BranchController(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        // GET: api/Branch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            log.Info("Fetching all branches");
            var branches = await _branchRepository.DisplayBranchDetails();
            log.Info($"Returned {branches.Count()} branches.");
            return Ok(branches);
        }

        // GET: api/Branch/{branchId}
        [HttpGet("{branchId}")]
        public async Task<ActionResult<Branch>> GetBranchById(int branchId)
        {
            log.Info($"Fetching details for branch ID: {branchId}");
            var branch = await _branchRepository.DisplayBranchById(branchId);
            if (branch == null)
            {
                log.Warn($"Branch with ID {branchId} not found.");
                return NotFound();
            }
            log.Info($"Returned details for branch ID: {branchId}");
            return Ok(branch);
        }
    }
}
