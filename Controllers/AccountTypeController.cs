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
    [Route("api/accountType")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {
        private readonly IAccountTypeRepository _accountTypeRepository;


        private static readonly ILog log = LogManager.GetLogger(typeof(AccountTypeController));

        public AccountTypeController(IAccountTypeRepository accountTypeRepository)
        {
            _accountTypeRepository = accountTypeRepository;
        }
        [HttpGet("allAccountTypes")]
        public async Task<ActionResult<IEnumerable<AccountType>>> GetAccountTypes()
        {
            log.Info("Fetching all account types");
            var accountTypes = await _accountTypeRepository.DisplayAccountType();
            log.Info($"Returned {accountTypes.Count()} account types.");
            return Ok(accountTypes);
        }

        [HttpGet("{accountTypeId}")]
        public async Task<ActionResult<string>> GetAccountTypeNameById(int accountTypeId)
        {
            log.Info($"Fetching account type name for account type ID: {accountTypeId}");
            var accountTypeName = await _accountTypeRepository.GetAccountTypeNameById(accountTypeId);

            if (string.IsNullOrEmpty(accountTypeName))
            {
                log.Warn($"Account type name not found for account type ID: {accountTypeId}");
                return NotFound($"No account type found for account type ID: {accountTypeId}");
            }

            log.Info($"Returned account type name for account type ID: {accountTypeId}");
            return Ok(accountTypeName);
        }
    }
}
