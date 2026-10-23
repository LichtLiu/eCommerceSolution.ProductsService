using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data;

public class ApplicationDbContext : DbContext
{
    // options(連線字串、資料庫種類)由 DI 傳進來,Step 4 才會設定
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // 一個 DbSet<T> = 一張資料表,表名預設用屬性名稱 "Products"
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
