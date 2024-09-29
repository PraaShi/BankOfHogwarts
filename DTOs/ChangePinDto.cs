using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.DTOs
{
    public class ChangePinDto
    {
        public string AccountId { get; set; }
        public string OldPin { get; set; }
        public string NewPin { get; set; }
        
    }
}
