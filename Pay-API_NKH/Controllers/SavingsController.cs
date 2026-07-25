using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pay_API_NKH.Data;
using Pay_API_NKH.DTOs;
using Pay_API_NKH.Models;

namespace Pay_API_NKH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SavingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/savings/open
        [HttpPost("open")]
        public async Task<IActionResult> OpenSaving([FromBody] OpenSavingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Tìm tài khoản nguồn
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);
            if (account == null)
                return NotFound(new { message = "Tài khoản nguồn không tồn tại." });

            // 2. Kiểm tra số dư
            if (account.AvailableBalance < request.Amount)
                return BadRequest(new { message = "Số dư không đủ để mở sổ tiết kiệm." });

            // 3. Xác định lãi suất theo kỳ hạn (annual percentage)
            decimal interestRate = GetInterestRateByTerm(request.TermMonths);

            // 4. Tạo bản ghi Savings
            var startDate = DateTime.Now;
            var maturityDate = startDate.AddMonths(request.TermMonths);

            var saving = new Savings
            {
                SavingId = Guid.NewGuid().ToString(),
                AccountNumber = account.AccountNumber,
                Amount = request.Amount,
                TermMonths = request.TermMonths,
                InterestRate = interestRate,
                StartDate = startDate,
                MaturityDate = maturityDate,
                AutoRenew = request.AutoRenew,
                IsActive = true
            };

            // 5. Trừ tiền từ tài khoản thanh toán
            account.AvailableBalance -= request.Amount;

            // 6. Tạo giao dịch ghi nhận việc chuyển tiền ra (âm)
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                AccountNumber = account.AccountNumber,
                Amount = -request.Amount,
                TransactionDate = DateTime.Now,
                Description = $"Mở sổ tiết kiệm (SavingId: {saving.SavingId})"
            };

            try
            {
                _context.Set<Savings>().Add(saving);
                _context.Transactions.Add(transaction);
                _context.Accounts.Update(account);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Mở sổ tiết kiệm thành công.",
                    saving = new
                    {
                        saving.SavingId,
                        saving.AccountNumber,
                        saving.Amount,
                        saving.TermMonths,
                        saving.InterestRate,
                        saving.StartDate,
                        saving.MaturityDate,
                        saving.AutoRenew
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi lưu dữ liệu.", error = ex.Message });
            }
        }
        // GET: /api/savings/rates
        [HttpGet("rates")]
        public IActionResult GetSavingsRates()
        {
            var rates = new[]
            {
        new { termMonths = 1, interestRate = 3.5 },
        new { termMonths = 2, interestRate = 3.7 },
        new { termMonths = 3, interestRate = 3.8 },
        new { termMonths = 6, interestRate = 4.8 },
        new { termMonths = 9, interestRate = 4.9 },
        new { termMonths = 12, interestRate = 5.2 },
        new { termMonths = 18, interestRate = 5.5 },
        new { termMonths = 24, interestRate = 5.8 },
        new { termMonths = 36, interestRate = 5.8 }
    };

            return Ok(rates);
        }

        private decimal GetInterestRateByTerm(int termMonths)
        {
            // Quy ước: trả về lãi suất hàng năm (%) tùy theo kỳ hạn
            if (termMonths <= 3)
                return 3.0m;
            if (termMonths <= 6)
                return 4.0m;
            if (termMonths <= 12)
                return 5.0m;
            if (termMonths <= 24)
                return 6.0m;
            return 7.0m; // up to 36 months
        }
    }
}
