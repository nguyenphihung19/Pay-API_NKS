using System;

namespace Pay_API_NKH.DTOs
{
    public class RegisterRequest
    {
        public string AccountNumber { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string CitizenId { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public decimal InitialBalance { get; set; } // Số dư ban đầu người dùng nạp vào
    }
}