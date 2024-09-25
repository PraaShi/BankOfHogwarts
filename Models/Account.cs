using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BankOfHogwarts.Models.Enums;
using System.Text.Json.Serialization;

namespace BankOfHogwarts.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        [Range(1000,9999)]
        public int PIN { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }


        [Required]
        [ForeignKey("AccountTypeId")]
        public int AccountTypeId { get; set; }
        public virtual AccountType? AccountType { get; set; }


        [Required]
        [ForeignKey("BranchId")]
        public int BranchId { get; set; }
        public virtual Branch? Branch { get; set; }


        //[Required]
        public string? AccountNumber { get; set; }  //need to be generated as a unique 10 digit number



        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }


        public int CIBILScore { get; set; } = 0; //need to be fixed 


        public DateTime CreatedAt { get; set; } = DateTime.Now;


        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountStatus Status { get; set; } = AccountStatus.PendingApproval;

        public int? BeneficiaryId { get; set; }

        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        [JsonIgnore]
        public ICollection<Loan>? Loans { get; set; } = new List<Loan>();

        [JsonIgnore]
        public ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();
    }
}
