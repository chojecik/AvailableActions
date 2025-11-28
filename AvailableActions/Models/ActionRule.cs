namespace AvailableActions.Models;

public class ActionRule
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> AllowedCardTypes { get; set; } = new();
    public Dictionary<string, string> AllowedCardStatuses { get; set; } = new();
}
