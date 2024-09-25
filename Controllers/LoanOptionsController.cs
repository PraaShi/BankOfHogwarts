using BankOfHogwarts.Models;
using BankOfHogwarts.Repositories;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankOfHogwarts.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoanOptionsController : ControllerBase
    {
        private readonly ILoanOptionsRepository _loanOptionsRepository;

        // Define the logger
        private static readonly ILog log = LogManager.GetLogger(typeof(LoanOptionsController));

        public LoanOptionsController(ILoanOptionsRepository loanOptionsRepository)
        {
            _loanOptionsRepository = loanOptionsRepository;
        }

        // Get all loan options
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanOptions>>> GetAllLoanOptions()
        {
            log.Info("Attempting to retrieve all loan options.");

            var loanOptions = await _loanOptionsRepository.GetAllLoanOptionsAsync();
            if (loanOptions == null || loanOptions.Count() == 0)
            {
                log.Warn("No loan options found.");
                return NotFound("No loan options available.");
            }

            log.Info($"Successfully retrieved {loanOptions.Count()} loan options.");
            return Ok(loanOptions);
        }

        // Get loan option by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanOptions>> GetLoanOptionById(int id)
        {
            log.Info($"Attempting to retrieve loan option with ID: {id}");

            var loanOption = await _loanOptionsRepository.GetLoanOptionByIdAsync(id);
            if (loanOption == null)
            {
                log.Warn($"Loan option with ID: {id} not found.");
                return NotFound($"Loan option with ID {id} not found.");
            }

            log.Info($"Successfully retrieved loan option with ID: {id}");
            return Ok(loanOption);
        }
    }
}
