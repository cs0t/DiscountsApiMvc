using Discounts.Application.Models;
using Discounts.Domain.Entities;
using Mapster;

namespace Discounts.API.Mapping;

public class MapsterRules : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Coupon to SellerSaleDto
        config.NewConfig<Coupon, SellerSaleDto>()
            .Map(dest => dest.CouponCode, src => src.Code)
            .Map(dest => dest.CouponStatus, src => src.Status.Name)
            .Map(dest => dest.CustomerUsername, src => src.Customer.UserName)
            .Map(dest => dest.PurchasedAt, src => src.PurchasedAt);
        
        //offer to offer details dto
        config.NewConfig<Offer,OfferDetailsDto>()
            .Map(dest => dest.RowVersion, src => Convert.ToBase64String(src.RowVersion))
            .Map(dest => dest.Status, src => src.Status.Name)  
            .Map(dest => dest.SellerUserName, src => src.Seller.UserName)
            .Map(dest => dest.Categories, src => src.Categories.Select(c => c.Name).ToList());
        
        //coupon to coupon details dto
        config.NewConfig<Coupon, CouponDetailsDto>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.CouponStatus, src => src.Status.Name)
            .Map(dest => dest.PurchasedAt, src => src.PurchasedAt)
            .Map(dest => dest.ExpirationDate, src => src.ExpirationDate)
            .Map(dest => dest.OfferTitle, src => src.Offer.Title)
            .Map(dest => dest.OfferDescription, src => src.Offer.Description)
            .Map(dest => dest.OriginalPrice, src => src.Offer.OriginalPrice)
            .Map(dest => dest.DiscountPrice, src => src.Offer.DiscountedPrice);
        
        //reservation to reservation details dto
        config.NewConfig<Reservation, ReservationDetailsDto>()
            .Map(dest => dest.ReservationId, src => src.Id)
            .Map(dest => dest.UserId, src => src.User.Id)
            .Map(dest => dest.UserName, src => src.User.UserName)
            .Map(dest => dest.UserEmail, src => src.User.Email)
            .Map(dest => dest.OfferId, src => src.Offer.Id)
            .Map(dest => dest.OfferTitle, src => src.Offer.Title)
            .Map(dest => dest.OfferDiscountedPrice, src => src.Offer.DiscountedPrice)
            .Map(dest => dest.OfferExpirationDate, src => src.Offer.ExpirationDate)
            .Map(dest => dest.SellerId, src => src.Offer.SellerId)
            .Map(dest => dest.SellerName, src => src.Offer.Seller.UserName) 
            .Map(dest => dest.ReservedAt, src => src.ReservedAt)
            .Map(dest => dest.ValidUntil, src => src.ValidUntil)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.Categories, src => src.Offer.Categories.Select(c => c.Name).ToList());
        
        //reservation to reservation dto
        config.NewConfig<Reservation, ReservationDto>()
            .Map(dest => dest.OfferTitle, src => src.Offer.Title);
        
        //customer offer state to dto
        config.NewConfig<CustomerOfferState, CustomerOfferStateDto>();
        
        //admin dtos
        config.NewConfig<Category, CategoryDto>();
        config.NewConfig<SystemSettings, SystemSettingsDto>();
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.Role, src => src.Role.Name);

    }
}