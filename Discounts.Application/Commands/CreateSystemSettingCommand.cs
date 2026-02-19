namespace Discounts.Application.Commands;

public class CreateSystemSettingCommand
{
    public string Key { get; set; } = null!;
    public string SettingValue { get; set; } = null!;
}

