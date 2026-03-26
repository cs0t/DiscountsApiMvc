using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries.Admin;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Admin.Offers;

public class GetOffersPagedAdminHandler(IOfferRepository offerRepository)
    : IRequestHandler<GetOffersPagedAdminQuery,PagedResult<Offer>>
{
    public Task<PagedResult<Offer>> Handle(GetOffersPagedAdminQuery query, CancellationToken ct = default)
    {
        return offerRepository.GetPagedAsync(query.pageNumber, query.pageSize, ct);
    }
}