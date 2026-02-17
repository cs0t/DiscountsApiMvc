namespace Discounts.Domain.Entities;

public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;

    public int OfferId { get; set; }
    public Offer Offer { get; set; } = null!;

    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;
    
    public int StatusId { get; set; }
    public CouponStatus Status { get; set; } = null!;
    
    public DateTime PurchasedAt { get; set; }
    
    public DateTime ExpirationDate { get; set; }
}