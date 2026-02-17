namespace Discounts.API.Requests.SellerRequests;

public class UpdateOfferRequest : CreateOfferRequest
{
    public int OfferId { get; init; }
}