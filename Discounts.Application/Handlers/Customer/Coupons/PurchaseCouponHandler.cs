using Discounts.Application.Commands.CustomerCommands.Coupons;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Customer.Coupons;

public class PurchaseCouponHandler
(ICurrentUserService currentUserService , IOfferRepository offerRepository, 
    ICouponRepository couponRepository, IReservationRepository reservationRepository) 
    : IRequestHandler<PurchaseCouponCommand>
{
    public async Task Handle(PurchaseCouponCommand request, CancellationToken ct = default)
    {
        var existingOffer = await offerRepository.GetWithDetailsByIdAsync(request.OfferId, ct);
        if (existingOffer is null)
        {
            throw new OfferNotFoundException($"Offer with id {request.OfferId} not found !");
        }

        if (existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
        {
            throw new ApplicationException("Only approved offers can be reserved and purchased !");
        }
        
        var customerId = currentUserService.UserId;
        
        //check if reservation already exists
        var existingReservation = await reservationRepository
            .GetActiveReservationByUserIdAndOfferIdAsync(customerId, request.OfferId, ct);
        if (existingReservation is null)
            throw new ApplicationException("User must have a reservation before purchasing a coupon !");

        if (existingReservation.ValidUntil < DateTime.UtcNow)
            throw new ApplicationException("Reservation has already expired !");
        
        //generate unique coupon code
        var couponCode = Guid.NewGuid().ToString().Substring(0, 11).ToUpper();
        var coupon = new Coupon
        {
            Code = couponCode,
            OfferId = request.OfferId,
            CustomerId = customerId,
            PurchasedAt = DateTime.UtcNow,
            ExpirationDate = existingOffer.ExpirationDate,
            StatusId = (int)CouponStatusesEnum.Active
        };
        await couponRepository.Add(coupon, ct);
        existingReservation.IsActive = false;
        existingReservation.CancelledAt = DateTime.UtcNow;
    }
}