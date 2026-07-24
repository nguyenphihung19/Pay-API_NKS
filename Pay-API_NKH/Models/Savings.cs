using System.ComponentModel.DataAnnotations;
namespace Pay_API_NKH.Models
{
    public class Savings
    {
        [Key]
        public string SavingId { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TermMonths { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public bool AutoRenew { get; set; }
        public bool IsActive { get;set; }

    }
}
