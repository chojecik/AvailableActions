namespace AvailableActions.Models.Models;

public class CardAction
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> AllowedCardTypes { get; set; } = [];
    public Dictionary<string, string> AllowedCardStatuses { get; set; } = [];
}
