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
        // 連線字串從 appsettings.json 的 ConnectionStrings:DefaultConnection 讀取
        // 注意:Oracle 官方套件的方法是 UseMySQL(SQL 全大寫)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!));

        // Scoped:Repository 注入 DbContext(Scoped),生命週期要一致
        services.AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}
