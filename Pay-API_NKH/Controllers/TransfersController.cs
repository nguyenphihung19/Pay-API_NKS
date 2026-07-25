using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pay_API_NKH.Data;
using Pay_API_NKH.DTOs;
using Pay_API_NKH.Models;

namespace Pay_API_NKH.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class TransfersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransfersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST /api/transfers/internal
        [HttpPost("internal")]
        public async Task<IActionResult> TransferInternal([FromBody] TransferInternalRequest req)
        {
            var fromAcc = await _context.Accounts.FindAsync(req.FromAccount);
            if (fromAcc == null)
                return Ok(new { status = "FAIL", message = "Tài khoản gửi không tồn tại" });

            // Tìm tài khoản nhận theo số tài khoản HOẶC số điện thoại
            var toAcc = await _context.Accounts.FirstOrDefaultAsync(a =>
                a.AccountNumber == req.ToAccountOrPhone || a.Phone == req.ToAccountOrPhone);

            if (toAcc == null)
                return Ok(new { status = "FAIL", message = "Tài khoản nhận không tồn tại" });

            // Quy tắc: số dư sau chuyển phải >= 50,000
            if (fromAcc.AvailableBalance - req.Amount < 50000)
                return Ok(new { status = "FAIL", message = "Số dư còn lại sau chuyển khoản phải >= 50,000 đ" });

            var txnId = "TXN" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var now = DateTime.Now;

            // Trừ tiền tài khoản gửi
            fromAcc.AvailableBalance -= req.Amount;
            // Cộng tiền tài khoản nhận
            toAcc.AvailableBalance += req.Amount;

            // Ghi log 2 giao dịch (gửi và nhận)
            // Ghi log giao dịch người gửi
            _context.Transactions.Add(new Transaction
            {
                TransactionId = txnId,
                AccountNumber = fromAcc.AccountNumber,
                Amount = -req.Amount,
                TransactionDate = now,
                BalanceAfter = fromAcc.AvailableBalance,
                Note = req.Note
            });

            // Ghi log giao dịch người nhận (SỬA Ở ĐÂY)
            _context.Transactions.Add(new Transaction
            {
                TransactionId = txnId + "_1", // <--- Thêm "_1" vào sau để ID không bị trùng trong DB
                AccountNumber = toAcc.AccountNumber,
                Amount = req.Amount,
                TransactionDate = now,
                BalanceAfter = toAcc.AvailableBalance,
                Note = req.Note
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "SUCCESS",
                transactionId = txnId,
                timestamp = now.ToString("yyyy-MM-ddTHH:mm:ss"),
                remainingBalance = fromAcc.AvailableBalance
            });
        }

        // POST /api/transfers/external
        [HttpPost("external")]
        public async Task<IActionResult> TransferExternal([FromBody] TransferExternalRequest req)
        {
            var fromAcc = await _context.Accounts.FindAsync(req.FromAccount);
            if (fromAcc == null)
                return Ok(new { status = "FAIL", message = "Tài khoản gửi không tồn tại" });

            if (fromAcc.AvailableBalance - req.Amount < 50000)
                return Ok(new { status = "FAIL", message = "Số dư còn lại sau chuyển khoản phải >= 50,000 đ" });

            var txnId = "TXN" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var now = DateTime.Now;

            fromAcc.AvailableBalance -= req.Amount;

            _context.Transactions.Add(new Transaction
            {
                TransactionId = txnId,
                AccountNumber = fromAcc.AccountNumber,
                Amount = -req.Amount,
                TransactionDate = now,
                BalanceAfter = fromAcc.AvailableBalance,
                Note = req.Note,
                ToBankCode = req.ToBankCode
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "SUCCESS",
                transactionId = txnId,
                timestamp = now.ToString("yyyy-MM-ddTHH:mm:ss"),
                remainingBalance = fromAcc.AvailableBalance
            });
        }
    }
}