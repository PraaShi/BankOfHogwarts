using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BankOfHogwarts.Models.Enums;
using System.Text.Json.Serialization;

namespace BankOfHogwarts.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        [StringLength(30)]
        [EmailAddress]
        public string Email { get; set; }   

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmployeePosition Position { get; set; } 

        [Required]
        [StringLength(25)]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
