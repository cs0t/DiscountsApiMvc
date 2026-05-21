using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries.Customer;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Customer.Coupons;

public class GetCustomerCouponsHandler(
    ICurrentUserService currentUserService,
    ICouponRepository couponRepository)
    : IRequestHandler<GetCustomerCouponsQuery, PagedResult<Coupon>>
{
    public Task<PagedResult<Coupon>> Handle(GetCustomerCouponsQuery request, CancellationToken ct = default)
    {
        var customerId = currentUserService.UserId;
        return couponRepository.GetByCustomerIdPagedAsync(customerId, request.PageNumber, request.PageSize, ct);
    }
}

