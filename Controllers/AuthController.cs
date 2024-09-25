/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankOfHogwarts.Models; // Add your models namespace
using System.Linq;

namespace BankOfHogwarts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly BankContext _context;

        public AuthController(IConfiguration configuration, BankContext context)
        {
            _config = configuration;
            _context = context;
        }

        // Validate method to check user credentials for Admin or Customer
        [NonAction]
        public Customer ValidateCustomer(string email, string password)
        {
            return _context.Customers.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        [NonAction]
        public Admin ValidateAdmin(string email, string password)
        {
            return _context.Admins.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        [NonAction]
        public Employee ValidateEmployee(string email, string password)
        {
            return _context.Employees.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate(string email, string password, string userType)
        {
            IActionResult response = Unauthorized();
            var token = string.Empty;

            if (userType.ToLower() == "customer")
            {
                var customer = ValidateCustomer(email, password);
                if (customer != null)
                {
                    token = GenerateJwtToken(customer.Email, customer.FirstName, "Customer");
                    return Ok(new { Token = token });
                }
            }
            else if (userType.ToLower() == "admin")
            {
                var admin = ValidateAdmin(email, password);
                if (admin != null)
                {
                    token = GenerateJwtToken(admin.Email, admin.AdminName, "Admin");
                    return Ok(new { Token = token });
                }
            }
            else if (userType.ToLower() == "employee")
            {
                var employee = ValidateEmployee(email, password);
                if (employee != null)
                {
                    token = GenerateJwtToken(employee.Email, employee.FirstName, "Employee");
                    return Ok(new { Token = token });
                }
            }
            return response;
        }

        // Generate JWT token
        private string GenerateJwtToken(string email, string name, string role)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            // Add user information and roles to the JWT token claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),  // Set the expiration time
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankOfHogwarts.Models; // Add your models namespace
using System.Linq;
using Microsoft.IdentityModel.Tokens;
//using System.Web.Http;

namespace BankOfHogwarts.Controllers
{
   // [Route("api/[controller]")]
    [ApiController]
    [Route("api/auth")] // Set RoutePrefix for the controller
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly BankContext _context;

        public AuthController(IConfiguration configuration, BankContext context)
        {
            _config = configuration;
            _context = context;
        }

        // Validate method to check user credentials for Admin or Customer
        [NonAction]
        public Customer ValidateCustomer(string email, string password)
        {
            return _context.Customers.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        [NonAction]
        public Admin ValidateAdmin(string email, string password)
        {
            return _context.Admins.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        [NonAction]
        public Employee ValidateEmployee(string email, string password)
        {
            return _context.Employees.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        // Route for authentication
        [AllowAnonymous]
        [HttpPost("login")] // Specific route for login
        public IActionResult Authenticate(string email, string password, string userType)
        {
            IActionResult response = Unauthorized();
            var token = string.Empty;

            if (userType.ToLower() == "customer")
            {
                var customer = ValidateCustomer(email, password);
                if (customer != null)
                {
                    token = GenerateJwtToken(customer.Email, customer.FirstName, "Customer");
                    return Ok(new { Token = token });
                }
            }
            else if (userType.ToLower() == "admin")
            {
                var admin = ValidateAdmin(email, password);
                if (admin != null)
                {
                    token = GenerateJwtToken(admin.Email, admin.AdminName, "Admin");
                    return Ok(new { Token = token });
                }
            }
            else if (userType.ToLower() == "employee")
            {
                var employee = ValidateEmployee(email, password);
                if (employee != null)
                {
                    token = GenerateJwtToken(employee.Email, employee.FirstName, "Employee");
                    return Ok(new { Token = token });
                }
            }
            return response;
        }

        // Generate JWT token
        private string GenerateJwtToken(string email, string name, string role)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            // Add user information and roles to the JWT token claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),  // Set the expiration time
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

