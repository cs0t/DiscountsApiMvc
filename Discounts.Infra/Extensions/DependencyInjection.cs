using Discounts.Application.Interfaces.JwtContracts;
using Discounts.Infra.Persistence;
using Discounts.Infra.Repositories;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.UnitOfWorkContracts;
using Discounts.Infra.Security;
using Discounts.Infra.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Discounts.Infra.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDbContext (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IOfferRepository, OfferRepository >();
        services.AddScoped<IOfferStatusRepository, OfferStatusRepository >();
        services.AddScoped<ICouponRepository, CouponRepository >();
        services.AddScoped<IReservationRepository, ReservationRepository >();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
}