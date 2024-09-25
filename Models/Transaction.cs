using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using BankOfHogwarts.Models.Enums;
using System.Text.Json.Serialization;

namespace BankOfHogwarts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public DateOnly TransactionDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);


        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType TransactionType { get; set; }    //withdraw,deposite,transfer

        public string Description { get; set; }


        [Column(TypeName = "decimal(15, 2)")]
        public decimal? Debit { get; set; } //The withdrwal amount and transfer amount will be noted here


        [Column(TypeName = "decimal(15, 2)")]
        public decimal? Credit { get; set; }    //Deposited money and loan awailed money will be added here


        [Column(TypeName = "decimal(15, 2)")]
        public decimal? UpdatedBalance { get; set; }


        [ForeignKey("AccountId")]
        public int? AccountId { get; set; }
        public Account Account { get; set; }
    }
}
