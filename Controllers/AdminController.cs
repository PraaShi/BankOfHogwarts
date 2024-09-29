using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;
using BankOfHogwarts.Repositories;
using Microsoft.AspNetCore.Authorization;
using log4net;
using BankOfHogwarts.DTOs;

namespace BankOfHogwarts.Controllers
{
    //[Authorize]
    [Route("api/account")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;


        private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // Manage Employees

        // GET: api/Admin/Employees
        //[Authorize(Roles = "Admin")]
        [HttpGet("Employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            log.Info("Fetching all employees...");
            var employees = await _adminRepository.GetAllEmployees();
            log.Info($"Returned {employees.Count()} employees.");
            return Ok(employees);
        }

        // GET: api/Admin/Employees/{id}
        //[Authorize(Roles = "Admin")]
        [HttpGet("Employees/{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            log.Info($"Fetching employee with ID: {id}");
            var employee = await _adminRepository.GetEmployeeById(id);
            if (employee == null)
            {
                log.Warn($"Employee with ID: {id} not found.");
                return NotFound();
            }
            log.Info($"Returned employee: {employee.FirstName} {employee.LastName}");
            return Ok(employee);
        }

        // POST: api/Admin/Employees
        //[Authorize(Roles = "Admin")]
        [HttpPost("Employees")]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            log.Info($"Adding new employee: {employee.FirstName} {employee.LastName}");
            var newEmployee = await _adminRepository.AddEmployee(employee);
            log.Info($"Employee added with ID: {newEmployee.EmployeeId}");
            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmployeeId }, newEmployee);
        }

        // PUT: api/Admin/Employees/{id}
        //[Authorize(Roles = "Admin")]
        [HttpPut("Employees/{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                log.Warn($"Employee ID mismatch for update. Request ID: {id}, Employee ID: {employee.EmployeeId}");
                return BadRequest();
            }

            log.Info($"Updating employee with ID: {id}");
            var updatedEmployee = await _adminRepository.UpdateEmployee(employee);
            log.Info($"Employee updated: {updatedEmployee.FirstName} {updatedEmployee.LastName}");
            return Ok(updatedEmployee);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("deactivate-account/{accountId}")]
        public async Task<IActionResult> DeactivateAccount(int accountId)
        {
            // Call the repository function to deactivate the account
            var result = await _adminRepository.DeactivateAccountAsync(accountId);

            if (result)
            {
                return Ok(new { message = "Account deactivated successfully." });
            }
            else
            {
                return NotFound(new { message = "Account not found or already inactive." });
            }
        }

        // DELETE: api/Admin/Employees/{id}
        //[Authorize(Roles = "Admin")]
        /*[HttpDelete("Employees/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            log.Info($"Attempting to delete employee with ID: {id}");

            try
            {
                await _adminRepository.DeleteEmployee(id);
                log.Info($"Employee with ID: {id} deleted successfully.");
                return NoContent(); // HTTP 204 No Content on successful deletion
            }
            catch (Exception ex)
            {
                log.Error($"Failed to delete employee with ID: {id}. Error: {ex.Message}");
                return NotFound(new { message = ex.Message }); // Return HTTP 404 Not Found if employee does not exist
            }
        }*/


        // Manage Customers

        // GET: api/Admin/Customers
        //[Authorize(Roles = "Admin")]
        [HttpGet("Customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            log.Info("Fetching all customers...");
            var customers = await _adminRepository.GetAllCustomers();
            log.Info($"Returned {customers.Count()} customers.");
            return Ok(customers);
        }

        // GET: api/Admin/Customers/{id}
        //[Authorize(Roles = "Admin")]
        [HttpGet("customers/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            log.Info($"Fetching customer with ID: {id}");
            var customer = await _adminRepository.GetCustomerById(id);
            if (customer == null)
            {
                log.Warn($"Customer with ID: {id} not found.");
                return NotFound();
            }
            log.Info($"Returned customer: {customer.FirstName} {customer.LastName}");
            return Ok(customer);
        }

        // POST: api/Admin/Customers
        //[Authorize(Roles = "Admin")]
      
        [HttpPost("addCustomers")]
        public async Task<ActionResult<Customer>> AddCustomer([FromBody] CustomerCreationDto customerDto)
        {
            log.Info($"Attempting to add new customer: {customerDto.FirstName} {customerDto.LastName}");

            try
            {
                var customer = new Customer
                {
                    FirstName = customerDto.FirstName,
                    MiddleName = customerDto.MiddleName,
                    LastName = customerDto.LastName,
                    Gender = customerDto.Gender,
                    ContactNumber = customerDto.ContactNumber,
                    Address = customerDto.Address,
                    DateOfBirth = customerDto.DateOfBirth.Value, 
                    AadharNumber = customerDto.AadharNumber,
                    Pan = customerDto.Pan,
                    Email = customerDto.Email,
                    Password = customerDto.Password
                };

                var newCustomer = await _adminRepository.AddCustomer(customer);

                log.Info($"Customer added successfully with ID: {newCustomer.CustomerId}");
                return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, newCustomer);
            }
            catch (Exception ex)
            {
                log.Error($"Error while adding customer. Exception: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while adding the customer" });
            }
        }


        // PUT: api/Admin/Customers/{id}
        //[Authorize(Roles = "Admin")]
        [HttpPut("updateCustomers/{id}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                log.Warn($"Customer ID mismatch for update. Request ID: {id}, Customer ID: {customer.CustomerId}");
                return BadRequest();
            }

            log.Info($"Updating customer with ID: {id}");
            var updatedCustomer = await _adminRepository.UpdateCustomer(customer);
            log.Info($"Customer updated: {updatedCustomer.FirstName} {updatedCustomer.LastName}");
            return Ok(updatedCustomer);
        }

        [HttpGet("dashboard-report")]
        public async Task<IActionResult> GetDashboardReport()
        {
            var report = await _adminRepository.GenerateAdminDashboardReport();
            return Ok(report);
        }

        [HttpPut("deactivate-customer/{customerId}")]
        public async Task<IActionResult> DeactivateCustomer(int customerId)
        {
            // Call the repository function to deactivate the customer
            var result = await _adminRepository.DeactivateCustomerAsync(customerId);

            if (result)
            {
                return Ok(new { message = "Customer deactivated successfully." });
            }
            else
            {
                return NotFound(new { message = "Customer not found or already inactive." });
            }
        }

        // DELETE: api/Admin/Customers/{id}
        //[Authorize(Roles = "Admin")]
        /*[HttpDelete("Customers/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            log.Info($"Deleting customer with ID: {id}");
            await _adminRepository.DeleteCustomer(id);
            log.Info($"Customer with ID: {id} deleted.");
            return NoContent();
        }*/
    }
}
