using BankOfHogwarts.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.DTOs
{
    public class AccountCreationDto
    {

        [Required]
        [Range(1000, 9999)]
        public int PIN { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }


        [Required]
        [ForeignKey("AccountTypeId")]
        public int AccountTypeId { get; set; }


        [Required]
        [ForeignKey("BranchId")]
        public int BranchId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }
    }
}
