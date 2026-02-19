namespace Discounts.API.Requests.AdminRequests;

public class CreateSystemSettingRequest
{
    public string Key { get; set; } = null!;
    public string SettingValue { get; set; } = null!;
}