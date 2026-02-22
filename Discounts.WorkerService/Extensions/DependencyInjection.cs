using Microsoft.Extensions.DependencyInjection;

namespace Discounts.WorkerService.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<CleanupService>();
        return services;
    }
}