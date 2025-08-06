using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Api.Utils;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddTransient<IProductsManagementService, ProductsManagementService>();
        services.AddTransient<IOrdersManagementService, OrdersManagementService>();
        services.AddScoped<IRepository, EfRepository>();
        services.AddSingleton<JwtTokenServices>();
        return services;
    }
}
