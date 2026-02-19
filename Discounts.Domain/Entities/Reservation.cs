namespace Discounts.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int OfferId { get; set; }
    public Offer Offer { get; set; } = null!;
    
    public DateTime ReservedAt { get; set; }
    
    public DateTime? ValidUntil { get; set; }
    public DateTime? CancelledAt { get; set; }
    public bool IsActive { get; set; }
}