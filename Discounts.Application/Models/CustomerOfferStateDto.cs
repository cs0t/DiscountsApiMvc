namespace Discounts.Application.Models;

public class CustomerOfferStateDto
{
    public OfferDetailsDto Offer { get; set; } = null!;
    public bool HasActiveReservation { get; set; }
    public DateTime? ReservationValidUntil { get; set; }
    public bool CanReserve { get; set; }
    public bool CanPurchase { get; set; }
}