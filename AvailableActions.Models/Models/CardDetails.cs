using AvailableActions.Models.Models.Enums;

namespace AvailableActions.Models.Models;

public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);
