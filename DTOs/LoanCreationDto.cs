using BankOfHogwarts.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.DTOs
{
    public class LoanCreationDto
    {
        [ForeignKey("loanOptionsId")]
        public int LoanTypeId { get; set; }


        [Required]
        [StringLength(250)]
        public string Purpose { get; set; } // Purpose of the loan
    }
}
