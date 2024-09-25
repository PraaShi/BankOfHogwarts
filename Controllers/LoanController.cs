using Microsoft.AspNetCore.Mvc;
using BankOfHogwarts.Repositories;
using System.Threading.Tasks;
using BankOfHogwarts.Models.Enums;

namespace BankOfHogwarts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanRepository _loanRepository;

        public LoanController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        // GET: api/loan/{loanId}
        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoanById(int loanId)
        {
            var loan = await _loanRepository.GetLoanById(loanId);
            if (loan == null)
            {
                return NotFound(new { message = "Loan not found." });
            }
            return Ok(loan);
        }
       
    }
}
