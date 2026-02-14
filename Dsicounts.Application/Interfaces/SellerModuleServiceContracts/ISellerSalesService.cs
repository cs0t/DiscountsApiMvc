using Dsicounts.Application.Models;

namespace Dsicounts.Application.Interfaces.SellerModuleServiceContracts;

public interface ISellerSalesService
{
    Task<List<SellerSaleDto>> GetSalesHistoryAsync(int sellerId, CancellationToken ct = default);
}