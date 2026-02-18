namespace Discounts.Domain.Entities;

public class SystemSettings
{
    //public int OfferEditingTimeLimitInHours { get; set; }
    //public int ReservationTimeLimitInHours { get; set; }
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public string SettingValue { get; set; } = null!;
}