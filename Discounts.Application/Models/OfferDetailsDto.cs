namespace Discounts.Application.Models;

public class OfferDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public int MaxQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public string Status { get; set; } = null!; 
    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? EditableUntil { get; set; }

    public int SellerId { get; set; }
    public string SellerUserName { get; set; } = null!;
    
    public string RowVersion { get; set; } = null!;

    public List<string> Categories { get; set; } = new List<string>();  
}