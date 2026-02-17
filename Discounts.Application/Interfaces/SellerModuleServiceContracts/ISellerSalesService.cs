using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.SellerModuleServiceContracts;

public interface ISellerSalesService
{
    Task<List<Coupon>> GetSalesHistoryAsync(int sellerId, CancellationToken ct = default);
}