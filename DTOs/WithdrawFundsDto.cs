using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.DTOs
{
    public class WithdrawFundsDto
    {
        public int AccountId { get; set; }  

        public decimal Amount { get; set; }

        public string Pin { get; set; }
    }
}
