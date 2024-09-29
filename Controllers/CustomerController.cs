using Microsoft.AspNetCore.Mvc;
using BankOfHogwarts.Models;
using BankOfHogwarts.Repositories;
using Microsoft.AspNetCore.Authorization;
using log4net;
using BankOfHogwarts.DTOs;

namespace BankOfHogwarts.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        private static readonly ILog log = LogManager.GetLogger(typeof(CustomerController));

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Customer customer)
        {
            log.Info("Attempting to register new customer.");
            var createdCustomer = await _customerRepository.CreateCustomer(customer);
            if (createdCustomer == null)
            {
                log.Warn("Customer registration failed.");
                return BadRequest("Unable to create customer.");
            }
            log.Info($"Customer registered successfully: {createdCustomer.Email}");
            return Ok(createdCustomer);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            log.Info($"Attempting login for customer with email: {email}");
            var customer = await _customerRepository.ValidateCustomer(email, password);
            if (customer == null)
            {
                log.Warn("Login attempt failed. Invalid credentials.");
                return Unauthorized("Invalid credentials.");
            }
            log.Info($"Customer logged in successfully: {email}");
            return Ok(customer);
        }

        [HttpGet("{customerId}/getaccounts")]
        public async Task<IActionResult> GetAccounts(int customerId)
        {
            log.Info($"Fetching accounts for customer ID: {customerId}");
            var accounts = await _customerRepository.DisplayAccounts(customerId);
            if (accounts == null || !accounts.Any())
            {
                log.Warn($"No accounts found for customer ID: {customerId}");
                return NotFound("No accounts found.");
            }
            log.Info($"Returned {accounts.Count()} accounts for customer ID: {customerId}");
            return Ok(accounts);
        }

        [HttpPost("{customerId}/createaccounts")]
        public async Task<IActionResult> CreateAccount(int customerId, [FromBody] AccountCreationDto accountDto)
        {
            log.Info($"Attempting to create account for customer ID: {customerId}");

            try
            {
                if (accountDto.CustomerId != customerId)
                {
                    return BadRequest(new { message = "Customer ID in the URL and the body do not match." });
                }

                var account = new Account
                {
                    CustomerId = accountDto.CustomerId,
                    AccountTypeId = accountDto.AccountTypeId,
                    BranchId = accountDto.BranchId,
                    Balance = accountDto.Balance,
                    PIN = accountDto.PIN
                };

                var createdAccount = await _customerRepository.CreateAccount(customerId, account);

                log.Info($"Account created for customer ID: {customerId}, pending approval.");
                return Ok(new { message = "Account created and pending approval", createdAccount });
            }
            catch (Exception ex)
            {
                log.Error($"Error while creating account for customer ID: {customerId}. Exception: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
