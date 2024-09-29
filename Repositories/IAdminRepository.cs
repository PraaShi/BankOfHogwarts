using System.Collections.Generic;
using System.Threading.Tasks;
using BankOfHogwarts.Models;

namespace BankOfHogwarts.Repositories
{
    public interface IAdminRepository
    {
        // Manage Employee
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<Employee?> GetEmployeeById(int id);
        Task<Employee> AddEmployee(Employee employee);
        Task<Employee> UpdateEmployee(Employee employee);
        Task<bool> DeactivateAccountAsync(int accountId);

        // Manage Customers
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<Customer?> GetCustomerById(int id);
        Task<Customer> AddCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task<bool> DeactivateCustomerAsync(int customerId);
        Task<AdminDashboardReport> GenerateAdminDashboardReport();
    }
}