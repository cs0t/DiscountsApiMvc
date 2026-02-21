using System.ComponentModel.DataAnnotations;

namespace Discounts.Domain.Entities;

public class Offer
{
    public int Id { get; set; }
    
    public string Title { get; set; } = null!;
    public string? Description { get; set; } 
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }


    public int MaxQuantity { get; set; }
    public int RemainingQuantity { get; set; }


    public int StatusId { get; set; }
    public OfferStatus Status { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DisabledAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? EditableUntil { get; set; }
    
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    
    public int SellerId { get; set; }
    public User Seller { get; set; } = null!;
    
    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
}