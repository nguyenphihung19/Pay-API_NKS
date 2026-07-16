using System;
using System.ComponentModel.DataAnnotations;

namespace Pay_API_NKH.Models
{
    public class Account
    {
        [Key]
        public string AccountNumber { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string CitizenId { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public decimal AvailableBalance { get; set; }
    }
}