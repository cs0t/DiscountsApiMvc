using System.Data;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.CustomerModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Application.Interfaces.UnitOfWorkContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.CustomerModuleServices;

public class CustomerService : ICustomerService
{
    private readonly IUserRepository _userRepository;
    private readonly IOfferRepository _offerRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUnitOfWork _unitOfWork;
    
    public CustomerService(
        IUserRepository userRepository, 
        IOfferRepository offerRepository, 
        ICouponRepository couponRepository,
        IReservationRepository reservationRepository,
        ISystemSettingsService settingsService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _offerRepository = offerRepository;
        _couponRepository = couponRepository;
        _reservationRepository = reservationRepository;
        _settingsService = settingsService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task PurchaseCouponAsync(int customerId, int offerId, CancellationToken ct = default)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existingOffer = await _offerRepository.GetWithDetailsByIdAsync(offerId, ct);
            if (existingOffer is null)
            {
                throw new OfferNotFoundException($"Offer with id {offerId} not found !");
            }

            if (existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
            {
                throw new ApplicationException("Only approved offers can be reserved and purchased !");
            }

            //check if reservation already exists
            var existingReservation = await _reservationRepository
                .GetActiveReservationByUserIdAndOfferIdAsync(customerId, offerId, ct);
            if (existingReservation is null)
                throw new ApplicationException("User must have a reservation before purchasing a coupon !");

            if (existingReservation.ValidUntil < DateTime.UtcNow)
                throw new ApplicationException("Reservation has already expired !");
            
            var existingUser = await _userRepository.GetWithRolesAsync(customerId, ct);
            
            if (existingUser is null)
            {
                throw new UserNotFoundException($"User with id {customerId} not found !");
            }
            
            if(existingUser.RoleId != (int)RoleEnum.Customer)
            {
                throw new ApplicationException("Only customers can purchase coupons !");
            }
        
            //generate unique coupon code
            var couponCode = Guid.NewGuid().ToString().Substring(0, 11).ToUpper();
            var coupon = new Coupon
            {
                Code = couponCode,
                OfferId = offerId,
                CustomerId = customerId,
                PurchasedAt = DateTime.UtcNow,
                ExpirationDate = existingOffer.ExpirationDate,
                StatusId = (int)CouponStatusesEnum.Active
            };
            await _couponRepository.Add(coupon, ct);
            existingReservation.IsActive = false;
            existingReservation.CancelledAt = DateTime.UtcNow;
        }, ct);
    }

    public async Task CreateReservationAsync(int customerId, int offerId, byte[] rowVersion, CancellationToken ct = default)
    {
        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingOffer = await _offerRepository.GetWithDetailsByIdAsync(offerId, ct);
                if (existingOffer is null)
                {
                    throw new OfferNotFoundException($"Offer with id {offerId} not found !");
                }

                if (existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
                {
                    throw new ApplicationException("Only approved offers can be reserved !");
                }

                if (existingOffer.ExpirationDate < DateTime.UtcNow)
                {
                    throw new ApplicationException("Offer has already expired !");
                }

                if (!existingOffer.RowVersion.SequenceEqual(rowVersion))
                {
                    throw new DBConcurrencyException("The offer was modified during reservation! Please try again!");
                }
                
                //check if reservation already exists
                var existingReservation = await _reservationRepository
                    .GetActiveReservationByUserIdAndOfferIdAsync(customerId, offerId, ct);
                if (existingReservation is not null && existingReservation.IsActive)
                    throw new ApplicationException("User already has an active reservation for this offer !");

                //check if offer has available coupons
                if (existingOffer.RemainingQuantity < 1)
                    throw new ApplicationException("No coupons available for this offer !");

                var existingUser = await _userRepository.GetWithRolesAsync(customerId, ct);

                if (existingUser is null)
                {
                    throw new UserNotFoundException($"User with id {customerId} not found !");
                }

                if (existingUser.RoleId != (int)RoleEnum.Customer)
                {
                    throw new ApplicationException("Only customers can create reservations !");
                }

                var validityHours =
                    await _settingsService.GetSettingValueByKeyAsync<int>(
                        SystemSettingNames.ReservationTimeLimitInHours,
                        ct);

                var now = DateTime.UtcNow;
                var reservation = new Reservation
                {
                    UserId = customerId,
                    OfferId = offerId,
                    ReservedAt = now,
                    ValidUntil = now.AddHours(validityHours),
                    IsActive = true
                };

                //decrease spots
                existingOffer.RemainingQuantity--;
                //create reservation
                await _reservationRepository.Add(reservation, ct);
            }, ct);
        }
        catch (DBConcurrencyException)
        {
            throw new ApplicationException("The offer was modified during reservation ! Please try again !");
        }
    }

    public async Task<PagedResult<Coupon>> GetCustomerCouponsAsync(int customerId, 
        int page=1, int pageSize=8,CancellationToken ct = default)
    {
        var existingUser = await _userRepository.GetWithRolesAsync(customerId, ct);
        if (existingUser is null)
        {
            throw new UserNotFoundException($"User with id {customerId} not found !");
        }
        if(existingUser.RoleId != (int)RoleEnum.Customer)
        {
            throw new ApplicationException("Only customers can view their coupons !");
        }
        var pagedResult = await _couponRepository.GetByCustomerIdPagedAsync(customerId, page, pageSize, ct);
        return pagedResult;
    }
    
    public async Task CancelReservationAsync(int reservationId, int customerId, CancellationToken ct = default)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existingReservation = await _reservationRepository.GetById(reservationId, ct);
            if (existingReservation is null)
            {
                throw new ApplicationException($"Reservation with id {reservationId} not found !");
            }

            if (existingReservation.UserId != customerId)
            {
                throw new ApplicationException("Users can only cancel their own reservations !");
            }

            if (!existingReservation.IsActive)
            {
                throw new ApplicationException("Only active reservations can be cancelled !");
            }

            existingReservation.IsActive = false;
            existingReservation.CancelledAt = DateTime.UtcNow;


            var existingOffer = await _offerRepository.GetById(existingReservation.OfferId, ct);
            if (existingOffer is null || existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
            {
                throw new ApplicationException("Associated offer not found or not approved !");
            }

            existingOffer.RemainingQuantity++;
        }, ct);
    }
    
    public async Task<PagedResult<Reservation>> GetCustomerReservationsAsync(int customerId, 
        int page=1, int pageSize=8,CancellationToken ct = default)
    {
        var existingUser = await _userRepository.GetWithRolesAsync(customerId, ct);
        if (existingUser is null)
        {
            throw new UserNotFoundException($"User with id {customerId} not found !");
        }
        if(existingUser.RoleId != (int)RoleEnum.Customer)
        {
            throw new ApplicationException("Only customers can view their reservations !");
        }
        var pagedResult = await _reservationRepository.GetActiveReservationsByUserIdAsync(customerId, page, pageSize, ct);
        return pagedResult;
    }

    public async Task<CustomerOfferState> GetOfferStateForCustomerAsync(int offerId, int customerId, CancellationToken ct = default)
    {
        //public OfferDetailsDto Offer { get; set; } = null!;
        //public bool HasActiveReservation { get; set; }
        //public DateTime? ReservationValidUntil { get; set; }
        //public bool CanReserve { get; set; }
        //public bool CanPurchase { get; set; }
        var existingOffer = await _offerRepository.GetWithDetailsByIdAsync(offerId, ct);
        if (existingOffer is null)        
            throw new OfferNotFoundException($"Offer with id {offerId} not found !");
        
        if(existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
            throw new ApplicationException("Only approved offers can be viewed !");
        
        var existingUser = await _userRepository.GetWithRolesAsync(customerId, ct);
        if (existingUser is null)
            throw new UserNotFoundException($"User with id {customerId} not found !");
        
        if(existingUser.RoleId != (int)RoleEnum.Customer)
            throw new ApplicationException("Only customers can view offer details !");
        
        var activeReservation = await _reservationRepository.GetActiveReservationByUserIdAndOfferIdAsync(customerId, offerId, ct);
        var hasActiveReservation = activeReservation is not null && activeReservation.IsActive;
        var reservationValidUntil = hasActiveReservation ? activeReservation!.ValidUntil : null;
        var canReserve = !hasActiveReservation && existingOffer.RemainingQuantity > 0 && existingOffer.ExpirationDate > DateTime.UtcNow;
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
    
    
    public async Task<PagedResult<Offer>> GetApprovedOffersAsync(OfferListQuery query,int customerId, CancellationToken ct = default)
    {
        var existingUser = await _userRepository.GetWithRolesAsync(customerId, ct);
        if (existingUser is null)
        {
            throw new UserNotFoundException($"User with id {customerId} not found !");
        }
        if(existingUser.RoleId != (int)RoleEnum.Customer)
        {
            throw new ApplicationException("Only customers can view their coupons !");
        }
        
        var pagedResult = await _offerRepository.GetApprovedActiveOffersAsync(query, ct);
        return pagedResult;
    }
}