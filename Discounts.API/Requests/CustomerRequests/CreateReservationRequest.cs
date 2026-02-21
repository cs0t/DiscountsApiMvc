namespace Discounts.API.Requests.CustomerRequests;

public class CreateReservationRequest
{
    public int OfferId { get; set; }
    public string RowVersion { get; set; } = null!;
}