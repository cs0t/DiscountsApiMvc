namespace Discounts.Domain.Entities;

public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;

    public int OfferId { get; set; }
    public Offer? Offer { get; set; }

    public int CustomerId { get; set; }
    public User? Customer { get; set; }
    
    public DateTime ExpirationDate { get; set; }
}