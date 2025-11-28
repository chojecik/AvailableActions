using AvailableActions.Models;

namespace AvailableActions.Services.Abstraction;

public interface ICardService
{
    Task<CardDetails?> GetCardDetails(string userId, string cardNumber);
}
