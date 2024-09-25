using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BankOfHogwarts.Models.Enums;
using System.Text.Json.Serialization;

namespace   BankOfHogwarts.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }


        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        public Account? Account { get; set; } // Navigation to Customer


        [ForeignKey("loanOptionsId")]
        public int LoanTypeId { get; set; }
        public LoanOptions? LoanOptions { get; set; }


        [ForeignKey("EmployeeId")] //Employee who review the loan
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }


        [Required]
        [StringLength(250)]
        public string Purpose { get; set; } // Purpose of the loan


        public DateTime ApplicationDate { get; set; } = DateTime.Now;


        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LoanApplicationStatus LoanApplicationStatus { get; set; } // Enum for application status


        public string? Remarks { get; set; }


        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LoanStatus LoanStatus { get; set; } // Enum for loan status

        public DateTime? ApprovedDate { get; set; }

        public DateTime? DisbursementDate { get; set; } // Date when the Amount was sent
    }
}
