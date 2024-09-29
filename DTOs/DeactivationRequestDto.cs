using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.DTOs
{
    public class DeactivationRequestDto
    {
        public int AccountId { get; set; }  

        public string Pin { get; set; }
    }
}
