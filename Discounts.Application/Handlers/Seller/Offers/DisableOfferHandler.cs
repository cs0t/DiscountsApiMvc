using Discounts.Application.Commands.SellerCommands.Offers;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Seller.Offers;

public class DisableOfferHandler(
    IOfferRepository offerRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<DisableOfferCommand>
{
    public async Task Handle(DisableOfferCommand request, CancellationToken ct = default)
    {
        var existingOffer = await offerRepository.GetById(request.Id, ct);
        if (existingOffer is null)
            throw new OfferNotFoundException($"Offer with id {request.Id} not found !");

        if (existingOffer.SellerId != currentUserService.UserId)
            throw new ForbiddenException("User is not authorized to disable this offer.");

        if (existingOffer.StatusId == (int)OfferStatusesEnum.Expired ||
            existingOffer.StatusId == (int)OfferStatusesEnum.Disabled)
            return;

        existingOffer.StatusId = (int)OfferStatusesEnum.Disabled;
        existingOffer.DisabledAt = DateTime.UtcNow;
        await offerRepository.SaveChangesAsync(ct);
    }
}