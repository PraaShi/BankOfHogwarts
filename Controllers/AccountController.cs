﻿using BankOfHogwarts.DTOs;
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
    [Route("api/accountActions")]
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

        /*[HttpPost("{id}/depositFunds")]
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
        }*/

        [HttpPost("{id}/depositFunds")]
        public async Task<IActionResult> DepositFunds(int id, [FromBody] DepositFundsDto depositFundsDto)
        {
            log.Info($"Attempting deposit for AccountId: {id} of amount: {depositFundsDto.Amount}");

            if (depositFundsDto == null)
            {
                log.Error("Deposit request data is missing.");
                return BadRequest("Invalid request data.");
            }

            if (id != depositFundsDto.AccountId)
            {
                log.Error($"Account ID mismatch. URL ID: {id}, DTO ID: {depositFundsDto.AccountId}");
                return BadRequest("Account ID in the URL does not match the Account ID in the request body.");
            }

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

                var transaction = await _accountRepository.DepositFund(id, depositFundsDto.Amount, depositFundsDto.Pin);
                log.Info($"Deposit successful for AccountId: {id}, Amount: {depositFundsDto.Amount}");

                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts Deposit Confirmation",
                    $"Dear {customerName}, your deposit of {depositFundsDto.Amount:C} has been successfully processed.");

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during deposit for AccountId: {id}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }



        /*[HttpPost("{id}/withdrawFunds")]
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
        }*/

        [HttpPost("{id}/withdrawFunds")]
        public async Task<IActionResult> WithdrawFunds(int id, [FromBody] WithdrawFundsDto withdrawFundsDto)
        {
            log.Info($"Attempting withdrawal for AccountId: {id}, Amount: {withdrawFundsDto.Amount}");

            if (withdrawFundsDto == null)
            {
                log.Error("Withdrawal request data is missing.");
                return BadRequest("Invalid request data.");
            }

            if (id != withdrawFundsDto.AccountId)
            {
                log.Error($"Account ID mismatch. URL ID: {id}, DTO ID: {withdrawFundsDto.AccountId}");
                return BadRequest("Account ID in the URL does not match the Account ID in the request body.");
            }

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

                var transaction = await _accountRepository.WithdrawFund(id, withdrawFundsDto.Amount, withdrawFundsDto.Pin);
                log.Info($"Withdrawal successful for AccountId: {id}, Amount: {withdrawFundsDto.Amount}");

                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                /*NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts Withdrawal Confirmation",
                    $"Dear {customerName}, your withdrawal of {withdrawFundsDto.Amount:C} has been successfully processed.");*/

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during withdrawal for AccountId: {id}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }


        /*[HttpPost("{id}/transferMoney")]
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
        }*/

        [HttpPost("{id}/transferMoney")]
        public async Task<IActionResult> TransferMoney(int id, [FromBody] TransferMoneyDto transferMoneyDto)
        {
            log.Info($"Attempting money transfer from AccountId: {id} to BeneficiaryId: {transferMoneyDto.BeneficiaryId}, Amount: {transferMoneyDto.Amount}");

            if (transferMoneyDto == null)
            {
                log.Error("Transfer request data is missing.");
                return BadRequest("Invalid request data.");
            }

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

                var transaction = await _accountRepository.TransferMoney(id, transferMoneyDto.BeneficiaryId, transferMoneyDto.Amount, transferMoneyDto.Pin);

                var beneficiaryName = await _accountRepository.GetBeneficiariesByAccountId(id);
                var bname = beneficiaryName.FirstOrDefault(x => x.BeneficiaryId == transferMoneyDto.BeneficiaryId);

                log.Info($"Money transfer successful from AccountId: {id} to Beneficiary Name: {bname.AccountName}, Amount: {transferMoneyDto.Amount}");

                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                /*NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts Money Transfer Confirmation",
                    $"Dear {customerName}, your transfer of {transferMoneyDto.Amount:C} to Beneficiary Name: {bname.AccountName} has been successfully processed.");*/

                return Ok(transaction);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during transfer for AccountId: {id}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }





        [HttpPost("{accountId}/change-pin")]
        public async Task<IActionResult> ChangePin(int accountId, [FromBody] ChangePinDto changePinDto)
        {
            log.Info($"Attempting to change PIN for AccountId: {accountId}");

            try
            {
                // Validate the incoming DTO data (optional, if using fluent validation or attributes)
                if (changePinDto == null)
                {
                    log.Error("ChangePinDto is null.");
                    return BadRequest("Invalid data received.");
                }

                // Display the account details
                var account = await _accountRepository.DisplayAccountDetails(accountId);
                if (account == null)
                {
                    log.Error($"Account with Id: {accountId} not found.");
                    return NotFound($"Account with Id: {accountId} not found.");
                }

                var customer = account.Customer;
                if (customer == null)
                {
                    log.Error($"Customer for AccountId: {accountId} not found.");
                    return NotFound($"Customer for AccountId: {accountId} not found.");
                }

                // Use the ChangePinDto values for the pin change operation
                var result = await _accountRepository.ChangePin(accountId, changePinDto.OldPin, changePinDto.NewPin);
                log.Info($"PIN change successful for AccountId: {accountId}");

                // Send a notification email
                string customerName = $"{customer.FirstName} {customer.LastName}";
                string customerEmail = customer.Email;

                /*NotifyUser.NotifyUserByEmail(
                    customerName,
                    customerEmail,
                    "Bank Of Hogwarts PIN Change Confirmation",
                    $"Dear {customerName}, your PIN has been successfully changed for your account (ID: {accountId}). If this was not you, please contact support immediately."
                );*/

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Error during PIN change for AccountId: {accountId}, Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                log.Error($"Invalid operation during PIN change for AccountId: {accountId}, Message: {ex.Message}");
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
        public async Task<IActionResult> RequestDeactivation(int accountId, [FromBody] DeactivationRequestDto deactivationRequestDto)
        {
            if (deactivationRequestDto == null)
            {
                log.Error("Deactivation request data is missing.");
                return BadRequest("Invalid request data.");
            }

            if (accountId != deactivationRequestDto.AccountId)
            {
                log.Error($"Account ID mismatch. URL ID: {accountId}, DTO ID: {deactivationRequestDto.AccountId}");
                return BadRequest("Account ID in the URL does not match the Account ID in the request body.");
            }

            if (string.IsNullOrWhiteSpace(deactivationRequestDto.Pin))
            {
                log.Error("PIN cannot be empty.");
                return BadRequest("PIN is required for deactivation request.");
            }

            try
            {
                // Call the repository method with the AccountId and Pin from the DTO
                var result = await _accountRepository.RequestDeactivation(deactivationRequestDto.AccountId, deactivationRequestDto.Pin);

                if (!result)
                {
                    log.Warn($"Deactivation request failed for AccountId: {accountId}");
                    return BadRequest("Deactivation request failed. Account may already be inactive or does not exist.");
                }

                log.Info($"Deactivation request submitted successfully for AccountId: {accountId}");
                return Ok("Deactivation request submitted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                log.Error($"Deactivation request failed for AccountId: {accountId}. Reason: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

    }
}
