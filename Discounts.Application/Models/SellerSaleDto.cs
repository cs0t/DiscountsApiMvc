using Discounts.Domain.Constants;

namespace Discounts.Application.Models;

public class SellerSaleDto
{
    public string CouponCode { get; set; } = null!;

    public string CustomerUsername { get; set; } = null!;

    public DateTime PurchasedAt { get; set; }

    public string CouponStatus { get; set; } = null!;
}