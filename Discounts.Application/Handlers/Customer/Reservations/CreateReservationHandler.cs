using System.Data;
using Discounts.Application.Commands.CustomerCommands.Reservations;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Customer.Reservations;

public class CreateReservationHandler(
    ICurrentUserService currentUserService, 
    IOfferRepository offerRepository, 
    IReservationRepository reservationRepository,
    ISystemSettingsService settingsService) : IRequestHandler<CreateReservationCommand>
{
    public async Task Handle(CreateReservationCommand request, CancellationToken ct = default)
    {
        try
        {
            var existingOffer = await offerRepository.GetWithDetailsByIdAsync(request.OfferId, ct);
            if (existingOffer is null)
            {
                throw new OfferNotFoundException($"Offer with id {request.OfferId} not found !");
            }
            
            if (existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
            {
                throw new ApplicationException("Only approved offers can be reserved !");
            }
            
            if (existingOffer.ExpirationDate < DateTime.UtcNow)
            {
                throw new ApplicationException("Offer has already expired !");
            }
            
            if (!existingOffer.RowVersion.SequenceEqual(request.RowVersion))
            {
                throw new DBConcurrencyException("The offer was modified during reservation! Please try again!");
            }
            
            int customerId = currentUserService.UserId;
            
            //check if reservation already exists
            var existingReservation = await reservationRepository
                .GetActiveReservationByUserIdAndOfferIdAsync(customerId, request.OfferId, ct);
            if (existingReservation is not null && existingReservation.IsActive)
                throw new ApplicationException("User already has an active reservation for this offer !");
            
            //check if offer has available coupons
            if (existingOffer.RemainingQuantity < 1)
                throw new ApplicationException("No coupons available for this offer !");

            var validityHours =
                await settingsService.GetSettingValueByKeyAsync<int>(
                    SystemSettingNames.ReservationTimeLimitInHours,
                    ct);
            
            var now = DateTime.UtcNow;
            var reservation = new Reservation
            {
                UserId = customerId,
                OfferId = request.OfferId,
                ReservedAt = now,
                ValidUntil = now.AddHours(validityHours),
                IsActive = true
            };
            
            //decrease spots
            existingOffer.RemainingQuantity--;
            //create reservation
            await reservationRepository.Add(reservation, ct);
        }
        catch (DBConcurrencyException)
        {
            throw new ApplicationException("The offer was modified during reservation ! Please try again !");
        }
    }
}