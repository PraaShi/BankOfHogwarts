using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;
using BankOfHogwarts.Repositories;
using Microsoft.AspNetCore.Authorization;
using log4net;
using BankOfHogwarts.Email;
using Microsoft.EntityFrameworkCore;
using BankOfHogwarts.Models.Enums;

namespace BankOfHogwarts.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IAccountRepository _accountRepository;

        private static readonly ILog log = LogManager.GetLogger(typeof(EmployeeController));

        public EmployeeController(IEmployeeRepository employeeRepository, ILoanRepository loanRepository, IAccountRepository accountRepository)
        {
            _employeeRepository = employeeRepository;
            _loanRepository = loanRepository;
            _accountRepository = accountRepository;
        }

        // Manage Account Requests (Approve/Reject)
        [HttpPost("manage-account-request/{accountId}")]
        public async Task<IActionResult> ManageAccountRequest(int accountId, [FromQuery] bool isApproved)
        {
            var result = await _employeeRepository.ManageAccountRequest(accountId, isApproved);
            if (!result)
            {
                log.Warn($"Account request management failed. AccountId: {accountId}");
                return NotFound(new { message = "Account not found or failed to update." });
            }

            var status = isApproved ? "approved" : "rejected";
            var account = await _accountRepository.DisplayAccountDetails(accountId);
            if (account == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            var customer = account.Customer;
            var name = customer.FirstName + customer.LastName;
            var email = customer.Email;
            if (customer == null)
            {
                log.Warn($"Customer not found for AccountId: {accountId}");
                return NotFound(new { message = "Customer not found." });
            }
            log.Info($"Account {accountId} has been {status} successfully.");

            try
            {
                string subject = $"Your Account Request has been {status}";
                string body = $"Dear {customer.FirstName} {customer.LastName},\n\n" +
                              $"Your account request for Account ID {accountId} has been {status}.\n\n" +
                              "Thank you for choosing our services.\n\nBest regards,\nBank of Hogwarts Team";

                NotifyUser.NotifyUserByEmail(name, email, subject, body);

                log.Info($"Email notification sent to {customer.Email} regarding account {status}.");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to send email notification to {customer.Email}. Error: {ex.Message}");
            }

            return Ok(new { message = $"Account has been {status} successfully." });
        }


        // Manage Loan Requests (Approve/Reject based on credit score)
        [HttpPost("manage-loan-request/{loanId}")]
        public async Task<IActionResult> ManageLoanRequest(int loanId, [FromQuery] bool isApproved, int employeeId)
        {
            log.Info($"Employee {employeeId} is managing loan request. LoanId: {loanId}, Approved: {isApproved}");

            var result = await _employeeRepository.ManageLoanRequest(loanId, isApproved, employeeId);
            if (!result)
            {
                log.Warn($"Loan request management failed. LoanId: {loanId}");
                return NotFound("Loan not found or failed to update.");
            }

            log.Info($"Loan {loanId} request processed successfully.");
            return Ok("Loan request processed successfully.");
        }

      
        // Disburse Loan Amount to Customer's Account
        [HttpPost("disburse-loan/{loanId}")]
        public async Task<IActionResult> DisburseLoanToAccount(int loanId, bool isApproved)
        {
            Console.WriteLine($"Attempting to disburse loan: loanId = {loanId}, isApproved = {isApproved}");


            var result = await _employeeRepository.DisburseLoanToAccount(loanId, isApproved);
            if (!result)
            {
                Console.WriteLine($"Loan {loanId} not found or disbursement failed.");
                return NotFound("Loan or account not found or failed to disburse.");
            }
            var loan = await _loanRepository.GetLoanById(loanId);
            if (loan == null || loan.Account == null || loan.Account.Customer == null)
            {
                Console.WriteLine($"Error fetching customer details for Loan {loanId}.");
                return NotFound("Customer details not found.");
            }

            var customer = loan.Account.Customer;
            var customerFullName = $"{customer.FirstName} {customer.LastName}";
            var customerEmail = customer.Email;

            string subject, body;
            if (isApproved)
            {
                subject = "Loan Approved - Disbursement Confirmation";
                body = $"Dear {customerFullName},\n\nWe are pleased to inform you that your loan (Loan ID: {loanId}) has been approved and the amount has been disbursed to your account.\n\nThank you for choosing our bank!";
            }
            else
            {
                subject = "Loan Application Rejected";
                body = $"Dear {customerFullName},\n\nWe regret to inform you that your loan application (Loan ID: {loanId}) has been rejected.\n\nThank you for choosing our bank!";
            }

            try
            {
                NotifyUser.NotifyUserByEmail(customerFullName, customerEmail, subject, body);
                Console.WriteLine($"Loan {loanId} disbursed successfully, and email notification sent to {customerEmail}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loan {loanId} disbursed successfully, but failed to send email. Error: {ex.Message}");
            }

            return Ok("Loan disbursed successfully.");
        }


        [HttpPost("{loanId}/close-loan")]
        public async Task<IActionResult> CloseLoan(int loanId)
        {
            log.Info($"Employee is attempting to close loan with LoanId: {loanId}");

            var result = await _employeeRepository.CloseLoans(loanId);

            if (!result)
            {
                log.Warn($"Failed to close loan. LoanId: {loanId} not found or close operation failed.");
                return BadRequest("Failed to close the loan. Loan not found.");
            }

            log.Info($"Loan with LoanId: {loanId} closed successfully.");

            var loan = await _loanRepository.GetLoanById(loanId);
            if (loan == null)
            {
                log.Warn($"Loan details could not be retrieved for LoanId: {loanId}");
                return NotFound(new { message = "Loan details not found after closure." });
            }

            var account = loan.Account;
            var customer = account?.Customer;
            var customername = customer.FirstName + " " + customer.LastName;
            var customeremail = customer.Email;

            if (customer == null)
            {
                log.Warn($"Customer details could not be found for LoanId: {loanId}, AccountId: {account?.AccountId}");
                return NotFound(new { message = "Customer not found for the loan." });
            }

            try
            {
                string subject = "Your Loan Has Been Successfully Closed";
                string body = $"Dear {customer.FirstName} {customer.LastName},\n\n" +
                              $"We are pleased to inform you that your loan with Loan ID {loanId} has been successfully closed.\n" +
                              $"Thank you for choosing our services, and we hope to continue serving you in the future.\n\n" +
                              "Best regards,\nBank of Hogwarts Team";

                NotifyUser.NotifyUserByEmail(customername, customeremail, subject, body);

                log.Info($"Email notification sent to {customer.Email} regarding loan closure.");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to send email notification to {customer.Email} for LoanId: {loanId}. Error: {ex.Message}");
            }

            return Ok("Loan closed successfully.");
        }


        // Display Transactions by Account ID
        [HttpGet("transactions/{accountId}")]
        public async Task<IActionResult> DisplayTransactionsByAccountID(int accountId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            log.Info($"Fetching transactions for AccountId: {accountId} from {startDate} to {endDate}");

            var transactions = await _employeeRepository.DisplayTransactionsByAccountID(accountId, startDate, endDate);
            log.Info($"Transactions fetched for AccountId: {accountId}");
            return Ok(transactions);
        }

        // Manage Account Deletion Request (Approve/Reject)
        [HttpDelete("manage-account-deletion/{accountId}")]
        public async Task<IActionResult> ManageAccountDeletionRequest(int accountId)
        {
            log.Info($"Attempting to process account deletion request. AccountId: {accountId}");

            var result = await _employeeRepository.ManageAccountDeletionRequest(accountId);
            if (!result)
            {
                log.Warn($"Account deletion request failed. AccountId: {accountId}");
                return NotFound("Account not found or failed to delete.");
            }

            log.Info($"Account {accountId} deletion request processed successfully.");
            return Ok("Account deletion request processed successfully.");
        }

        [HttpGet("financial-report")]
        public async Task<IActionResult> GetFinancialReport()
        {
            log.Info("Generating financial report.");

            var report = await _employeeRepository.GenerateFinancialReports();
            log.Info("Financial report generated successfully.");
            return Ok(report);
        }


        [HttpPost("{accountId}/deactivate")]
        public async Task<IActionResult> DeactivateAccount(int accountId)
        {
            try
            {
                // Attempt to retrieve account details
                var account = await _accountRepository.DisplayAccountDetails(accountId);

                log.Info($"Employee is attempting to deactivate account with AccountId: {accountId}");

                var result = await _employeeRepository.DeactivateAccount(accountId);


                if (!result)
                {
                    log.Warn($"Account deactivation failed for AccountId: {accountId}. Possible active loans or account does not exist.");
                    return BadRequest("Account deactivation failed. There might be active loans, or the account does not exist.");
                }


                log.Info($"Account with AccountId: {accountId} deactivated successfully.");

                // Check if the account is inactive or not approved
                /*if (account.Status != AccountStatus.Active)
                {
                    log.Warn($"Account {accountId} is either inactive or not approved.");
                    return BadRequest("Account cannot be deactivated as it is either inactive or not approved.");
                }*/

                

                // Get customer information
                var customer = account?.Customer;
                if (customer == null)
                {
                    log.Warn($"Customer details not found for AccountId: {accountId}");
                    return NotFound(new { message = "Customer not found for the deactivated account." });
                }

                // Prepare customer details for the email
                var customername = $"{customer.FirstName} {customer.LastName}";
                var customeremail = customer.Email;

                // Send email notification to the customer
                try
                {
                    string subject = "Your Account Has Been Deactivated";
                    string body = $"Dear {customer.FirstName} {customer.LastName},\n\n" +
                                  $"We would like to inform you that your account with Account ID {accountId} has been deactivated successfully.\n" +
                                  $"If you have any questions or concerns, feel free to contact us.\n\n" +
                                  "Best regards,\nBank of Hogwarts Team";

                    // Send the email using the NotifyUser service
                    NotifyUser.NotifyUserByEmail(customername, customeremail, subject, body);

                    log.Info($"Email notification sent to {customer.Email} regarding account deactivation.");
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to send email notification to {customer.Email} for AccountId: {accountId}. Error: {ex.Message}");
                }

                return Ok("Account deactivated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                // Handle the exception thrown by DisplayAccountDetails
                log.Error($"Error while attempting to deactivate account {accountId}: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                log.Error($"Unexpected error while deactivating account {accountId}: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


    }
}
