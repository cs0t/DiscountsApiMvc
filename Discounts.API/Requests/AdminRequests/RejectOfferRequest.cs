namespace Discounts.API.Requests.AdminRequests;

public class RejectOfferRequest
{
    public int OfferId { get; set; }
    public string Reason { get; set; } = null!;
}