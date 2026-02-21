namespace Discounts.Application.Commands;

public class CreateReservationCommand
{
    public int OfferId { get; set; }
    public string RowVersion { get; set; } = null!;
}