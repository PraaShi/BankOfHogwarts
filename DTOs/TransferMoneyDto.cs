using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.DTOs
{
    public class TransferMoneyDto
    {
        public int BeneficiaryId { get; set; }

        public decimal Amount { get; set; }

        public string Pin { get; set; }
    }
}
