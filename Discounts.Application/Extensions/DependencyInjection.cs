using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Application.Services.AuthServices;
using Discounts.Application.Services.SellerModuleServices;
using Discounts.Application.Services.SystemSettingsServices;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISellerDashboardService, SellerDashboardService>();
        services.AddScoped<IOfferManagementService, OfferManagementService>();
        services.AddScoped<ISellerSalesService, SellerSalesService>();
        services.AddScoped<ISystemSettingsService, SystemSettingsService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}