namespace Discounts.API.Requests.AdminRequests;

public class UpdateSystemSettingRequest
{
    public int Id { get; set; }
    public string NewSettingValue { get; set; } = null!;
}