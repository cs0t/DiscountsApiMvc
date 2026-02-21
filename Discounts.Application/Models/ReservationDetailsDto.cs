namespace Discounts.Application.Models;

public class ReservationDetailsDto
{
    public int ReservationId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    
    public int OfferId { get; set; }
    public string OfferTitle { get; set; } = null!;
    public decimal OfferDiscountedPrice { get; set; }
    public DateTime OfferExpirationDate { get; set; }
    
    public int SellerId { get; set; }
    public string SellerName { get; set; } = null!;

    public DateTime ReservedAt { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsActive { get; set; }
    
    public List<string> Categories { get; set; } = new List<string>();
}