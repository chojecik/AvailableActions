using AvailableActions.Api.Controllers;
using AvailableActions.Business.Services.Abstraction;
using AvailableActions.Models.Models;
using AvailableActions.Models.Models.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AvailableActions.Api.Tests.Controllers;

public class CardsControllerTests
{
    private readonly Mock<ICardService> _cardServiceMock = new();
    private readonly Mock<IRulesService> _rulesServiceMock = new();
    private readonly CardsController _controller;

    public CardsControllerTests()
    {
        _controller = new CardsController(_cardServiceMock.Object, _rulesServiceMock.Object, new NullLogger<CardsController>());
    }

    [Fact]
    public async Task GetAllowedActions_ReturnsBadRequest_WhenUserIdOrCardNumberEmpty()
    {
        // Act
        var result1 = await _controller.GetAllowedActions("", "Card1");
        var result2 = await _controller.GetAllowedActions("User1", "   ");

        // Assert
        result1.Should().BeOfType<BadRequestObjectResult>();
        result2.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAllowedActions_ReturnsNotFound_WhenUserNotFound()
    {
        // Arrange
        _cardServiceMock.Setup(s => s.UserExists("User11")).ReturnsAsync(false);

        // Act
        var result = await _controller.GetAllowedActions("User11", "Card1");

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAllowedActions_ReturnsNotFound_WhenCardNotFound()
    {
        // Arrange
        _cardServiceMock.Setup(s => s.GetCardDetails("User1", "Card1")).ReturnsAsync((CardDetails?)null);

        // Act
        var result = await _controller.GetAllowedActions("User1", "Card1");

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAllowedActions_ReturnsOk_WithAllowedActions()
    {
        // Arrange
        var card = new CardDetails("Card11", CardType.Prepaid, CardStatus.Closed, IsPinSet: false);
        var expectedActions = new List<string> { "ACTION3", "ACTION4", "ACTION9" };

        _cardServiceMock.Setup(s => s.UserExists("User1")).ReturnsAsync(true);
        _cardServiceMock.Setup(s => s.GetCardDetails("User1", "Card11")).ReturnsAsync(card);
        _rulesServiceMock.Setup(s => s.GetAllowedActions(card)).Returns(expectedActions);

        // Act
        var result = await _controller.GetAllowedActions("User1", "Card11") as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        result.Should().BeOfType<OkObjectResult>();
        result.Value.Should().BeEquivalentTo(expectedActions);
    }
}
