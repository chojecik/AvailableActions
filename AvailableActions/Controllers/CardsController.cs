using AvailableActions.Api.Responses;
using AvailableActions.Business.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace AvailableActions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly IRulesService _rulesService;
    private readonly ILogger<CardsController> _logger;


    public CardsController(ICardService cardService, IRulesService rulesService, ILogger<CardsController> logger)
    {
        _cardService = cardService;
        _rulesService = rulesService;
        _logger = logger;
    }


    [HttpGet("{userId}/{cardNumber}/actions")]
    public async Task<IActionResult> GetAllowedActions(string userId, string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(cardNumber))
            return BadRequest(new { error = "userId and cardNumber are required" });

        try
        {
            if (!await _cardService.UserExists(userId))
                return NotFound(new { error = "User not found" });

            var card = await _cardService.GetCardDetails(userId, cardNumber);

            if (card == null)
                return NotFound(new { error = "Card not found" });

            var actions = _rulesService.GetAllowedActions(card);

            return Ok(new AllowedActionsResponse
            {
                UserId = userId,
                Card = card
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAllowedActions");

            return StatusCode(500, new { error = "internal_server_error", detail = ex.Message });
        }
    }
}
