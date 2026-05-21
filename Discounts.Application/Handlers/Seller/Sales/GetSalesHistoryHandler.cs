using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Queries.Seller;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Seller.Sales;

public class GetSalesHistoryHandler(
    ICouponRepository couponRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetSalesHistoryQuery, List<Coupon>>
{
    public async Task<List<Coupon>> Handle(GetSalesHistoryQuery request, CancellationToken ct = default)
    {
        return await couponRepository.GetBySellerIdAsync(currentUserService.UserId, ct);
    }
}

