using System.ComponentModel.DataAnnotations;

namespace Pay_API_NKH.DTOs
{
    public class OpenSavingRequest
    {
        [Required(ErrorMessage = "Số tài khoản nguồn không được để trống")]
        public string AccountNumber { get; set; }

        [Required]
        [Range(10000, double.MaxValue, ErrorMessage = "Số tiền gửi tối thiểu là 10,000 VND")]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, 36, ErrorMessage = "Kỳ hạn từ 1 đến 36 tháng")]
        public int TermMonths { get; set; }
        public bool AutoRenew { get; set; } = false;
    }
}
