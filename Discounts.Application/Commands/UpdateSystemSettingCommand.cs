namespace Discounts.Application.Commands;

public class UpdateSystemSettingCommand
{
    public int Id { get; set; }
    public string NewSettingValue { get; set; } = null!;
}

