using Discounts.Domain.Entities;

namespace Dsicounts.Application.Interfaces.RepositoryContracts;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Coupon>> GetByCustomerIdAsync(int customerId,CancellationToken ct = default);
    Task<List<Coupon>> GetByOfferIdAsync(int offerId, CancellationToken ct = default);
    Task<int> GetCouponCountForSellerAsync(int sellerId, CancellationToken ct = default);
    Task<decimal> GetTotalIncomeFromCouponsForSellerAsync(int sellerId, CancellationToken ct = default);
}