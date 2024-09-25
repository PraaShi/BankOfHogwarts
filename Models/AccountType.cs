using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.Models
{
    public class AccountType
    {
        [Key]
        public int AccountTypeId { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountTypeName { get; set; } //Savings, Salary ,Business
    }
}
