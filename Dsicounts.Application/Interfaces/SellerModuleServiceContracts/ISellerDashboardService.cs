using Dsicounts.Application.Models;

namespace Dsicounts.Application.Interfaces.SellerModuleServiceContracts;

public interface ISellerDashboardService
{
    Task<SellerDashboardStats> GetDashboardStatsAsync(int sellerId, CancellationToken ct = default);
}