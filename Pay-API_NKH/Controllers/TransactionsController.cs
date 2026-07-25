using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pay_API_NKH.Data;
using Pay_API_NKH.Models;

namespace Pay_API_NKH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<IActionResult> GetTransactions( // <--- SỬA: Đổi kiểu trả về thành Task<IActionResult>
            [FromQuery] string? accountNumber,             // <--- THÊM: Nhận tham số accountNumber từ query
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.Transactions.AsQueryable();

            // <--- THÊM: Lọc theo accountNumber nếu người dùng truyền vào
            if (!string.IsNullOrEmpty(accountNumber))
            {
                query = query.Where(t => t.AccountNumber == accountNumber);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value.AddDays(1));
            }

            // <--- SỬA/THÊM: Chọn đúng các trường và định dạng camelCase chuẩn ví dụ đề bài
            var result = await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new
                {
                    transactionId = t.TransactionId,
                    accountNumber = t.AccountNumber,
                    amount = t.Amount,
                    transactionDate = t.TransactionDate,
                    note = t.Note
                })
                .ToListAsync();

            return Ok(result); // <--- SỬA: Trả về Ok(result)
        }
    }
}