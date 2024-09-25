using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankOfHogwarts.DTOs
{
    public class BenificiaryCreationDto
    {
        [StringLength(50)]
        public string AccountName { get; set; }


        [StringLength(20)]
        public string AccountNumber { get; set; }


        [ForeignKey("BranchId")]
        public int BranchId { get; set; }
    }
}
