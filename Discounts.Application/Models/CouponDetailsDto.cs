namespace Discounts.Application.Models;

public class CouponDetailsDto
{
    public string Code { get; set; } = null!;
    public string OfferTitle { get; set; } = null!;
    public string OfferDescription { get; set; } = null!;
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public string CouponStatus { get; set; } = null!;
    public DateTime PurchasedAt { get; set; }
    public DateTime ExpirationDate { get; set; }
}