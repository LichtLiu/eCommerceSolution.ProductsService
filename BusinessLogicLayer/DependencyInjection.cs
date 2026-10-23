using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Validators;          // ← 新增
using FluentValidation;                        // ← 新增
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ProductMappingProfile).Assembly));

        services.AddScoped<IProductsService, ProductsService>();

        // 掃描 BLL 組件,註冊所有 AbstractValidator<T>      ← 新增
        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();  // ← 新增

        return services;
    }
}
