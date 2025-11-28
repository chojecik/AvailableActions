using AvailableActions.Models.Models;

namespace AvailableActions.Api.Responses;

public class AllowedActionsResponse
{
    public string UserId { get; set; }

    public CardDetails Card { get; set; }
}
