using AvailableActions.Models.Models;

namespace AvailableActions.Business.Services.Abstraction;

public interface ICardService
{
    Task<CardDetails?> GetCardDetails(string userId, string cardNumber);
    Task<bool> UserExists(string userId);
}
