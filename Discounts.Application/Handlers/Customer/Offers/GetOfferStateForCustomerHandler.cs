using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries.Customer;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Customer.Offers;

public class GetOfferStateForCustomerHandler(
    ICurrentUserService currentUserService,
    IOfferRepository offerRepository,
    IReservationRepository reservationRepository)
    : IRequestHandler<GetOfferStateForCustomerQuery, CustomerOfferState>
{
    public async Task<CustomerOfferState> Handle(GetOfferStateForCustomerQuery request, CancellationToken ct = default)
    {
        var existingOffer = await offerRepository.GetWithDetailsByIdAsync(request.OfferId, ct);
        if (existingOffer is null)
            throw new OfferNotFoundException($"Offer with id {request.OfferId} not found !");

        if (existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
            throw new ApplicationException("Only approved offers can be viewed !");

        var customerId = currentUserService.UserId;

        var activeReservation = await reservationRepository
            .GetActiveReservationByUserIdAndOfferIdAsync(customerId, request.OfferId, ct);

        var hasActiveReservation = activeReservation is not null && activeReservation.IsActive;
        var reservationValidUntil = hasActiveReservation ? activeReservation!.ValidUntil : null;
        var canReserve = !hasActiveReservation
                         && existingOffer.RemainingQuantity > 0
                         && existingOffer.ExpirationDate > DateTime.UtcNow;
        var canPurchase = hasActiveReservation && reservationValidUntil > DateTime.UtcNow;

        return new CustomerOfferState
        {
            Offer = existingOffer,
            HasActiveReservation = hasActiveReservation,
            ReservationValidUntil = reservationValidUntil,
            CanReserve = canReserve,
            CanPurchase = canPurchase
        };
    }
}


