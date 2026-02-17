using Discounts.Application.Models;

namespace Discounts.Application.Interfaces.SellerModuleServiceContracts;

public interface ISellerDashboardService
{
    Task<SellerDashboardStats> GetDashboardStatsAsync(int sellerId, CancellationToken ct = default);
}