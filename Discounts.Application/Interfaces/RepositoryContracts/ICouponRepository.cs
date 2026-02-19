using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon> AddAndReturnAsync(Coupon entity, CancellationToken ct = default);
    Task<Coupon> UpdateAndReturnAsync(Coupon entity, CancellationToken ct = default);
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);

    Task<PagedResult<Coupon>> GetByCustomerIdPagedAsync(int customerId, int page = 1, int pageSize = 8,
        CancellationToken ct = default);
    Task<List<Coupon>> GetByOfferIdAsync(int offerId, CancellationToken ct = default);
    Task<List<Coupon>> GetBySellerIdAsync(int sellerId, CancellationToken ct = default);
    Task<int> GetCouponCountForSellerAsync(int sellerId, CancellationToken ct = default);
    Task<decimal> GetTotalIncomeFromCouponsForSellerAsync(int sellerId, CancellationToken ct = default);
}