using BankOfHogwarts.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankOfHogwarts.Models
{
    public class LoanOptions
    {
        [Key]
        public int LoanTypeId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LoanType LoanType { get; set; }


        public decimal LoanAmount { get; set; }

    
        public decimal InterestRate { get; set; } 


        public int Tenure { get; set; } // Duration in months
    }
}
