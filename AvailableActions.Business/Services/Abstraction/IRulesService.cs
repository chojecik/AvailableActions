using AvailableActions.Models.Models;

namespace AvailableActions.Business.Services.Abstraction;

public interface IRulesService
{
    IEnumerable<string> GetAllowedActions(CardDetails card);
}
