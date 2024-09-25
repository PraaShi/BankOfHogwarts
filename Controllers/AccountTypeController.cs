using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;
using BankOfHogwarts.Repositories;
using Microsoft.AspNetCore.Authorization;
using log4net;
using log4net.Config;


namespace BankOfHogwarts.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {
        private readonly IAccountTypeRepository _accountTypeRepository;


        private static readonly ILog log = LogManager.GetLogger(typeof(AccountTypeController));

        public AccountTypeController(IAccountTypeRepository accountTypeRepository)
        {
            _accountTypeRepository = accountTypeRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountType>>> GetAccountTypes()
        {
            log.Info("Fetching all account types");
            var accountTypes = await _accountTypeRepository.DisplayAccountType();
            log.Info($"Returned {accountTypes.Count()} account types.");
            return Ok(accountTypes);
        }
    }
}
