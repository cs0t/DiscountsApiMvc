namespace Discounts.Domain.Entities;

public class CouponStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}