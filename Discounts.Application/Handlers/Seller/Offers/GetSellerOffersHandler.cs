using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Application.Queries.Seller;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Seller.Offers;

public class GetSellerOffersHandler(
    IOfferRepository offerRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetSellerOffersQuery, PagedResult<Offer>>
{
    public async Task<PagedResult<Offer>> Handle(GetSellerOffersQuery request, CancellationToken ct = default)
    {
        var query = new OfferListQuery
        {
            PriceStart = request.PriceStart,
            PriceEnd = request.PriceEnd,
            CategoryIds = request.CategoryIds,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return await offerRepository.GetBySellerAsync(query, currentUserService.UserId, ct);
    }
}