namespace Discounts.Application.Models;

public class SystemSettingsDto
{
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public string SettingValue { get; set; } = null!;
}