using AvailableActions.Models.Enums;

namespace AvailableActions.Models;

public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);
