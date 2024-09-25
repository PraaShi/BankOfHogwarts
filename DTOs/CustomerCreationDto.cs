using BankOfHogwarts.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankOfHogwarts.DTOs
{
    public class CustomerCreationDto
    {

        [Required]
        [StringLength(40)]
        public string FirstName { get; set; }


        [StringLength(40)]
        public string? MiddleName { get; set; }   

        [Required]
        [StringLength(40)]
        public string LastName { get; set; }


        [StringLength(10)]
        [Required]
        public string Gender { get; set; }


        [Required]
        [StringLength(10)]
        public string ContactNumber { get; set; }


        [Required]
        public string Address { get; set; }

        [Required]
        public DateOnly? DateOfBirth { get; set; }


        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string AadharNumber { get; set; }


        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string Pan { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
