namespace Discounts.Application.Commands;

public class RejectOfferCommand
{
    public int OfferId { get; set; }
    public string Reason { get; set; } = null!;
}