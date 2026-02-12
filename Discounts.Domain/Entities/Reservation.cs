namespace Discounts.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public int OfferId { get; set; }
    public Offer? Offer { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime ReservedUntil { get; set; }
    public bool IsActive { get; set; }
}