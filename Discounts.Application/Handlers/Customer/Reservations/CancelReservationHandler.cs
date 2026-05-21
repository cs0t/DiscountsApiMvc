using Discounts.Application.Commands.CustomerCommands.Reservations;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Customer.Reservations;

public class CancelReservationHandler(
    ICurrentUserService currentUserService,
    IReservationRepository reservationRepository,
    IOfferRepository offerRepository)
    : IRequestHandler<CancelReservationCommand>
{
    public async Task Handle(CancelReservationCommand request, CancellationToken ct = default)
    {
        var existingReservation = await reservationRepository.GetById(request.Id, ct);
        if (existingReservation is null)
            throw new ApplicationException($"Reservation with id {request.Id} not found !");

        var customerId = currentUserService.UserId;
        if (existingReservation.UserId != customerId)
            throw new ApplicationException("Users can only cancel their own reservations !");

        if (!existingReservation.IsActive)
            throw new ApplicationException("Only active reservations can be cancelled !");

        existingReservation.IsActive = false;
        existingReservation.CancelledAt = DateTime.UtcNow;

        var existingOffer = await offerRepository.GetById(existingReservation.OfferId, ct);
        if (existingOffer is null || existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
            throw new ApplicationException("Associated offer not found or not approved !");

        existingOffer.RemainingQuantity++;
    }
}


