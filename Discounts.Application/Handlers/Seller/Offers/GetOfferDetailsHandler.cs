using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Queries.Seller;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Seller.Offers;

public class GetOfferDetailsHandler(
    IOfferRepository offerRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetOfferDetailsSellerQuery, Offer>
{
    public async Task<Offer> Handle(GetOfferDetailsSellerQuery request, CancellationToken ct = default)
    {
        var offer = await offerRepository.GetWithDetailsByIdAsync(request.OfferId, ct);
        if (offer is null)
            throw new OfferNotFoundException($"Offer with id {request.OfferId} not found !");

        if (offer.SellerId != currentUserService.UserId)
            throw new ForbiddenException("User does not have permission to view this offer !");

        return offer;
    }
}