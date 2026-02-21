namespace Discounts.Application.Models;

public class ReservationDto
{
    public int Id { get; set; }
    public int OfferId { get; set; }
    public string OfferTitle { get; set; } = null!;
    public DateTime ReservedAt { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsActive { get; set; }
}

