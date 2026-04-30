using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Application.Queries.Customer;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Customer.Offers;

public class GetApprovedOffersHandler(IOfferRepository offerRepository)
    : IRequestHandler<GetApprovedOffersQuery, PagedResult<Offer>>
{
    public Task<PagedResult<Offer>> Handle(GetApprovedOffersQuery request, CancellationToken ct = default)
    {
        var query = new OfferListQuery
        {
            PriceStart = request.PriceStart,
            PriceEnd = request.PriceEnd,
            CategoryIds = request.CategoryIds,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
        return offerRepository.GetApprovedActiveOffersAsync(query, ct);
    }
}

