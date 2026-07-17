using System.ComponentModel.DataAnnotations;

namespace Pay_API_NKH.Models
{
    public class Transaction
    {
        [Key]
        public string TransactionId { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}