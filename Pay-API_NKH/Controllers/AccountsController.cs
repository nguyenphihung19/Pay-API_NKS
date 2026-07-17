using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pay_API_NKH.Data; // Đảm bảo thư mục chứa ApplicationDbContext là Data
using Pay_API_NKH.Models;
using Pay_API_NKH.DTOs;
using System;
using System.Threading.Tasks;

namespace Pay_API_NKH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/accounts/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Kiểm tra các trường dữ liệu bắt buộc không được để trống
            if (string.IsNullOrEmpty(request.AccountNumber) ||
                string.IsNullOrEmpty(request.AccountHolder) ||
                string.IsNullOrEmpty(request.Phone) ||
                string.IsNullOrEmpty(request.CitizenId))
            {
                return BadRequest(new { message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });
            }

            // 2. Ràng buộc: Số dư ban đầu tối thiểu phải từ 100,000đ
            if (request.InitialBalance < 100000)
            {
                return BadRequest(new { message = "Số dư ban đầu tối thiểu để mở tài khoản phải từ 100,000đ." });
            }

            // 3. Ràng buộc: Căn cước công dân (CitizenId) còn hạn hay không
            if (request.ExpiryDate <= DateTime.Now)
            {
                return BadRequest(new { message = "Căn cước công dân đã hết hạn. Không thể mở tài khoản ngân hàng." });
            }

            // 4. Ràng buộc: Kiểm tra xem số tài khoản này đã có ai đăng ký chưa
            var isAccountExist = await _context.Accounts.AnyAsync(a => a.AccountNumber == request.AccountNumber);
            if (isAccountExist)
            {
                return BadRequest(new { message = "Số tài khoản này đã tồn tại trên hệ thống." });
            }

            // 5. Nếu vượt qua tất cả các kiểm tra, tiến hành tạo thực thể mới và lưu database
            var newAccount = new Account
            {
                AccountNumber = request.AccountNumber,
                AccountHolder = request.AccountHolder,
                Phone = request.Phone,
                CitizenId = request.CitizenId,
                ExpiryDate = request.ExpiryDate,
                AvailableBalance = request.InitialBalance
            };

            try
            {
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đăng ký tài khoản thành công!", account = newAccount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống khi lưu dữ liệu.", error = ex.Message });
            }
        }
        // thêm dòng này
        // GET: api/accounts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id) // Sửa int thành string để nhận Số tài khoản
        {
            // 1. Tìm tài khoản trong Database theo AccountNumber
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == id);

            // 2. Nếu không tìm thấy, trả về mã lỗi 404 Not Found kèm thông báo
            if (account == null)
            {
                return NotFound(new { message = $"Không tìm thấy tài khoản với số tài khoản = {id}" });
            }

            // 3. Nếu tìm thấy, trả về thông tin chi tiết tài khoản với mã 200 OK
            return Ok(new
            {
                account.AccountNumber,
                account.AccountHolder,
                account.Phone,
                account.CitizenId,
                account.ExpiryDate,
                account.AvailableBalance
            });
        }
    }
}