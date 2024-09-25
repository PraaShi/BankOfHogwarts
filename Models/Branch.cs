using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }


        [Required]
        [StringLength(50)]
        public string BranchName { get; set; } 


        public string BranchAddress { get; set; }


        [StringLength(50)]
        public string BankName { get; set; }


        [StringLength(11)]
        public string ContactNumber { get; set; }


        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string IFSCCode { get; set; } 


        [StringLength(50)]
        public string City { get; set; }


        [StringLength(50)]
        public string State { get; set; }


        [StringLength(10)]
        public string PostalCode { get; set; }
    }
}
