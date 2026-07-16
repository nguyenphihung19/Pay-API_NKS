using Microsoft.EntityFrameworkCore;
using Pay_API_NKH.Models;

namespace Pay_API_NKH.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; } = null!;
    }
}