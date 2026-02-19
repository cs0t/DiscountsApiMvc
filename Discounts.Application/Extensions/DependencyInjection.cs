using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.CustomerModuleContracts;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Application.Services.AdminModuleServices;
using Discounts.Application.Services.AuthServices;
using Discounts.Application.Services.CustomerModuleServices;
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
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IOfferManagementAdminService, OfferManagementAdminService>();
        services.AddScoped<ISystemSettingsManagementService, SystemSettingsManagementService>();
        services.AddScoped<ICategoryManagementService, CategoryManagementService>();
        
        return services;
    }
}