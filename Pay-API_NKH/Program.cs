// 1. TẤT CẢ CÁC DÒNG USING PHẢI ĐẶT Ở TRÊN CÙNG FILE
using Microsoft.EntityFrameworkCore;
using Pay_API_NKH.Data;

var builder = WebApplication.CreateBuilder(args);

// 2. Cấu hình kết nối SQL Server (Đặt dưới builder)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();