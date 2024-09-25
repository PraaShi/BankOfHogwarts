using BankOfHogwarts.DTOs;
using BankOfHogwarts.Email;
using BankOfHogwarts.Models;
using BankOfHogwarts.Models.Enums;
using BankOfHogwarts.Repositories;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankOfHogwarts.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        private static readonly ILog log = LogManager.GetLogger(typeof(AccountController));

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet("{id}/getAccountDetails")]
        public async Task<IActionResult> GetAccountDetails(int id)
        {
            log.Info($"Fetching account details for AccountId: {id}");
            var account = await _accountRepository.DisplayAccountDetails(id);

            if (account == null)
            {
                log.Warn($"Account with AccountId: {id} not found.");
                return NotFound();
            }

            log.Info($"Account details retrieved successfully for AccountId: {id}");
            return Ok(account);
        }

        [HttpGet("{id}/getTransactions")]
        public async Task<IActionResult> GetTransactions(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            log.Info($"Fetching transactions for AccountId: {id} from {startDate} to {endDate}");
            var transactions = await _accountRepository.DisplayTransactions(id, startDate, endDate);
            log.Info($"Returned {transactions.Count()} transactions for AccountId: {id}");
            return Ok(transactions);
        }

        [HttpPost("{id}/depositFunds")]
        public async Task<IActionResult> DepositFunds(int id, [FromQuery] decimal amount, [FromQuery] string pin)
        {
            log.Info($"Attempting deposit for AccountId: {id} of amount: {amount}");
            try
            {
                var account = await _accountRepository.DisplayAccountDetails(id);
                if (account == null)
                {
                    log.Error($"Account with Id: {id} not found.");
                    return NotFound($"Account with Id: {id} not found.");
                }
                var customer = account.Customer;
                if (customer == null)
                {
                    log.Error($"Customer for AccountId: {id} not found.");
                    return NotFound($"Customer for AccountId: {id} not found.");
                }

                var transaction = await _accountRepository.DepositFund(id, amount, pin);
                log.Info($"Deposit successful for AccountId: {id}, Amount: {amount}");

                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts Deposit Confirmation",
                    $"Dear {customerName}, your deposit of {amount:C} has been successfully processed.");

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during deposit for AccountId: {id}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("{id}/withdrawFunds")]
        public async Task<IActionResult> WithdrawFunds(int id, [FromQuery] decimal amount, [FromQuery] string pin)
        {
            log.Info($"Attempting withdrawal for AccountId: {id}, Amount: {amount}");
            try
            {
                var account = await _accountRepository.DisplayAccountDetails(id);
                if (account == null)
                {
                    log.Error($"Account with Id: {id} not found.");
                    return NotFound($"Account with Id: {id} not found.");
                }
                var customer = account.Customer;
                if (customer == null)
                {
                    log.Error($"Customer for AccountId: {id} not found.");
                    return NotFound($"Customer for AccountId: {id} not found.");
                }

                var transaction = await _accountRepository.WithdrawFund(id, amount, pin);
                log.Info($"Withdrawal successful for AccountId: {id}, Amount: {amount}");

                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts Withdrawal Confirmation",
                    $"Dear {customerName}, your withdrawal of {amount:C} has been successfully processed.");

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during withdrawal for AccountId: {id}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/transferMoney")]
        public async Task<IActionResult> TransferMoney(int id, [FromQuery] int beneficiaryId, [FromQuery] decimal amount, [FromQuery] string pin)
        {
            log.Info($"Attempting money transfer from AccountId: {id} to BeneficiaryId: {beneficiaryId}, Amount: {amount}");
            try
            {
                var account = await _accountRepository.DisplayAccountDetails(id);
                if (account == null)
                {
                    log.Error($"Account with Id: {id} not found.");
                    return NotFound($"Account with Id: {id} not found.");
                }

                var customer = account.Customer;
                if (customer == null)
                {
                    log.Error($"Customer for AccountId: {id} not found.");
                    return NotFound($"Customer for AccountId: {id} not found.");
                }

                var transaction = await _accountRepository.TransferMoney(id, beneficiaryId, amount, pin);
                var benificiaryname = await _accountRepository.GetBeneficiariesByAccountId(id);
                var bname = benificiaryname.FirstOrDefault(x=>x.BeneficiaryId==beneficiaryId);
                log.Info($"Money transfer successful from AccountId: {id} to Beneficiary Name: {bname.AccountName}, Amount: {amount}");

                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts Money Transfer Confirmation",
                    $"Dear {customerName}, your transfer of {amount:C} to Beneficiary Name: {bname.AccountName} has been successfully processed.");

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during transfer for AccountId: {id}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/change-pin")]
        public async Task<IActionResult> ChangePin(int id, [FromQuery] string oldPin, [FromQuery] string newPin)
        {
            log.Info($"Attempting to change PIN for AccountId: {id}");
            try
            {
                var account = await _accountRepository.DisplayAccountDetails(id);
                if (account == null)
                {
                    log.Error($"Account with Id: {id} not found.");
                    return NotFound($"Account with Id: {id} not found.");
                }
                var customer = account.Customer;
                if (customer == null)
                {
                    log.Error($"Customer for AccountId: {id} not found.");
                    return NotFound($"Customer for AccountId: {id} not found.");
                }

                var result = await _accountRepository.ChangePin(id, oldPin, newPin);
                log.Info($"PIN change successful for AccountId: {id}");

                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts PIN Change Confirmation",
                    $"Dear {customerName}, your PIN has been successfully changed for your account (ID: {id}). If this was not you, please contact support immediately.");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during PIN change for AccountId: {id}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("{id}/add-beneficiary")]
        public async Task<IActionResult> AddBeneficiary(int id, [FromBody] BenificiaryCreationDto beneficiaryDto)
        {
            log.Info($"Adding new beneficiary for AccountId: {id}");

            try
            {
                var beneficiary = new Beneficiary
                {
                    AccountId = id,
                    AccountName = beneficiaryDto.AccountName,
                    AccountNumber = beneficiaryDto.AccountNumber,
                    BranchId = beneficiaryDto.BranchId
                };

                var newBeneficiary = await _accountRepository.AddBeneficiary(id, beneficiary);

                log.Info($"Beneficiary added successfully for AccountId: {id}, BeneficiaryId: {newBeneficiary.AccountId}");

                return CreatedAtAction(nameof(GetAccountDetails), new { id = newBeneficiary.AccountId }, newBeneficiary);
            }
            catch (Exception ex)
            {
                log.Error($"Error while adding beneficiary for AccountId: {id}. Exception: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/apply-loan")]
        public async Task<IActionResult> ApplyLoan(int id, [FromBody] LoanCreationDto loanDto)
        {
            log.Info($"Attempting to apply for a loan for AccountId: {id}");

            try
            {
                var loan = new Loan
                {
                    AccountId = id, 
                    LoanTypeId = loanDto.LoanTypeId, 
                    Purpose = loanDto.Purpose, 

                    LoanApplicationStatus = LoanApplicationStatus.UnderReview,
                    Remarks = "Application received",
                    LoanStatus = LoanStatus.Pending,
                    ApplicationDate = DateTime.Now, 
                    ApprovedDate = null, 
                    DisbursementDate = null, 
                    EmployeeId = null 
                };

                // Pass the Loan object to the repository
                var newLoan = await _accountRepository.ApplyLoan(id, loan);

                if (newLoan == null)
                {
                    log.Warn($"Loan application failed for AccountId: {id}");
                    return BadRequest(new { message = "Loan could not be created" });
                }

                log.Info($"Loan applied successfully for AccountId: {id}, LoanId: {newLoan.LoanId}");
                return CreatedAtAction(nameof(GetAccountDetails), new { id = newLoan.AccountId }, newLoan);
            }
            catch (Exception ex)
            {
                log.Error($"Error while applying for a loan for AccountId: {id}. Exception: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while applying for the loan" });
            }
        }




        [HttpGet("{id}/loan-history")]
        public async Task<IActionResult> LoanHistory(int id)
        {
            log.Info($"Fetching loan history for AccountId: {id}");
            var loans = await _accountRepository.LoanHistory(id);
            log.Info($"Returned loan history for AccountId: {id}, Loans count: {loans.Count()}");
            return Ok(loans);
        }

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            log.Info($"Attempting to delete AccountId: {id}");
            var result = await _accountRepository.DeleteAccount(id);

            if (!result)
            {
                log.Warn($"AccountId: {id} not found for deletion");
                return NotFound();
            }

            log.Info($"AccountId: {id} deleted successfully");
            return NoContent();
        }*/

        [HttpGet("{accountId}/beneficiaries")]
        public async Task<IActionResult> GetBeneficiariesByAccountId(int accountId)
        {
            log.Info($"Fetching beneficiaries for AccountId: {accountId}");
            var beneficiaries = await _accountRepository.GetBeneficiariesByAccountId(accountId);
            log.Info($"Returned beneficiaries for AccountId: {accountId}, Count: {beneficiaries.Count()}");
            return Ok(beneficiaries);
        }

        [HttpPost("{accountId}/request-deactivation")]
        public async Task<IActionResult> RequestDeactivation(int accountId)
        {
            var result = await _accountRepository.RequestDeactivation(accountId);

            if (!result)
                return BadRequest("Deactivation request failed. Account may already be inactive or does not exist.");

            return Ok("Deactivation request submitted successfully.");
        }
    }
}
