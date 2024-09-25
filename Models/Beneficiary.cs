using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankOfHogwarts.Models
{
    public class Beneficiary
    {
        [Key]
        public int BeneficiaryId { get; set; }

        [StringLength(50)]
        public string AccountName { get; set; }


        [StringLength(20)]
        public string AccountNumber { get; set; }

        
        [ForeignKey("BranchId")]
        public int BranchId { get; set; }

        //[JsonIgnore]
        public virtual Branch? Branch { get; set; }



        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        public virtual Account? Account { get; set; } 

    }
}
