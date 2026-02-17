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
            .Map(dest => dest.Status, src => src.Status.Name)
            .Map(dest => dest.CustomerUsername, src => src.Customer.UserName)
            .Map(dest => dest.PurchasedAt, src => src.PurchasedAt);
    }
}