using AvailableActions.Models;

namespace AvailableActions.Services.Abstraction;

public interface IRulesService
{
    IEnumerable<string> GetAllowedActions(CardDetails card);
}
