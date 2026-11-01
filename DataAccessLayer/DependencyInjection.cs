using DataAccessLayer.Data;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer;

public static class DependencyInjection
{
    /// <summary>
    /// 註冊 DAL 的服務:DbContext(MySQL)與 Repository。
    /// </summary>
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // 連線字串含密碼,不放在 appsettings.json(會被打包進 Docker Image)。
        // 本機開發:dotnet user-secrets;容器 / 正式環境:環境變數 ConnectionStrings__DefaultConnection
        // 讀不到就在啟動時丟出明確的錯誤(fail fast),不要等到第一個請求才失敗
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Missing connection string 'ConnectionStrings:DefaultConnection'. " +
                "Set it with 'dotnet user-secrets' (local) or the environment variable " +
                "'ConnectionStrings__DefaultConnection' (container).");

        // 注意:Oracle 官方套件的方法是 UseMySQL(SQL 全大寫)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySQL(connectionString));

        // Scoped:Repository 注入 DbContext(Scoped),生命週期要一致
        services.AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}
