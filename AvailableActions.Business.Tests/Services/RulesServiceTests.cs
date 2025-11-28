using AvailableActions.Business.Services;
using AvailableActions.Models.Models;
using AvailableActions.Models.Models.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace AvailableActions.Business.Tests.Services;

public class RulesServiceTests
{
    [Fact]
    public void GetAllowedActions_WorksAsExpected_ForPrepaidfClosed()
    {
        // Arrange
        var tmp = CreateTempRulesFile(SampleRulesJson);
        var logger = new NullLogger<RulesService>();
        var expected = new[] { "ACTION_3", "ACTION_4", "ACTION_9" };

        // Act
        var svc = new RulesService(tmp, logger);
        var allowed = svc.GetAllowedActions(new CardDetails("C1", CardType.Prepaid, CardStatus.Closed, IsPinSet: false)).ToList();

        // Assert
        allowed.Should().BeEquivalentTo(expected);

        // cleanup
        File.Delete(tmp);
    }

    [Fact]
    public void GetAllowedActions_WorksAsExpected_ForCreditBlocked()
    {
        // Arrange
        var tmp = CreateTempRulesFile(SampleRulesJson);
        var logger = new NullLogger<RulesService>();
        var expected = new[] { "ACTION_3", "ACTION_4", "ACTION_5", "ACTION_6", "ACTION_7", "ACTION_8", "ACTION_9" };

        // Act
        var svc = new RulesService(tmp, logger);
        var allowed = svc.GetAllowedActions(new CardDetails("C1", CardType.Credit, CardStatus.Blocked, IsPinSet: true)).ToList();

        // Assert
        allowed.Should().BeEquivalentTo(expected);

        // cleanup
        File.Delete(tmp);
    }

    private static string CreateTempRulesFile(string jsonContent)
    {
        var tempFilePath = Path.GetTempFileName();
        File.WriteAllText(tempFilePath, jsonContent);
        return tempFilePath;
    }

    private const string SampleRulesJson = @"
    {
      ""actions"": [
        {
          ""name"": ""ACTION_3"",
          ""allowedCardTypes"": { ""Prepaid"": ""allowed"", ""Debit"": ""allowed"", ""Credit"": ""allowed"" },
          ""allowedCardStatuses"": { ""Ordered"": ""allowed"", ""Inactive"": ""allowed"", ""Active"": ""allowed"", ""Restricted"": ""allowed"", ""Blocked"": ""allowed"", ""Expired"": ""allowed"", ""Closed"": ""allowed"" }
        },
        {
          ""name"": ""ACTION_4"",
          ""allowedCardTypes"": { ""Prepaid"": ""allowed"", ""Debit"": ""allowed"", ""Credit"": ""allowed"" },
          ""allowedCardStatuses"": { ""Ordered"": ""allowed"", ""Inactive"": ""allowed"", ""Active"": ""allowed"", ""Restricted"": ""allowed"", ""Blocked"": ""allowed"", ""Expired"": ""allowed"", ""Closed"": ""allowed"" }
        },
        {
          ""name"": ""ACTION_5"",
          ""allowedCardTypes"": { ""Prepaid"": ""disabled"", ""Debit"": ""disabled"", ""Credit"": ""allowed"" },
          ""allowedCardStatuses"": { ""Ordered"": ""allowed"", ""Inactive"": ""allowed"", ""Active"": ""allowed"", ""Restricted"": ""allowed"", ""Blocked"": ""allowed"", ""Expired"": ""allowed"", ""Closed"": ""allowed"" }
        },
        {
          ""name"": ""ACTION_6"",
          ""allowedCardTypes"": { ""Prepaid"": ""allowed"", ""Debit"": ""allowed"", ""Credit"": ""allowed"" },
          ""allowedCardStatuses"": { ""Ordered"": ""allowed_if_pin_set"", ""Inactive"": ""allowed_if_pin_set"", ""Active"": ""allowed_if_pin_set"", ""Restricted"": ""disabled"", ""Blocked"": ""allowed_if_pin_set"", ""Expired"": ""disabled"", ""Closed"": ""disabled"" }
        },
        {
          ""name"": ""ACTION_7"",
          ""allowedCardTypes"": { ""Prepaid"": ""allowed"", ""Debit"": ""allowed"", ""Credit"": ""allowed"" },
          ""allowedCardStatuses"": { ""Ordered"": ""allowed_if_pin_not_set"", ""Inactive"": ""allowed_if_pin_not_set"", ""Active"": ""allowed_if_pin_not_set"", ""Restricted"": ""disabled"", ""Blocked"": ""allowed_if_pin_set"", ""Expired"": ""disabled"", ""Closed"": ""disabled"" }
        },
        {
          ""name"": ""ACTION_8"",
          ""allowedCardTypes"": { ""Prepaid"": ""allowed"", ""Debit"": ""allowed"", ""Credit"": ""allowed"" },
          ""allowedCardStatuses"": { ""Ordered"": ""allowed"", ""Inactive"": ""allowed"", ""Active"": ""allowed"", ""Restricted"": ""disabled"", ""Blocked"": ""allowed"", ""Expired"": ""disabled"", ""Closed"": ""disabled"" }
        },
        {
          ""name"": ""ACTION_9"",
          ""allowedCardTypes"": { ""Prepaid"": ""allowed"", ""Debit"": ""allowed"", ""Credit"": ""allowed"" },
          ""allowedCardStatuses"": { ""Ordered"": ""allowed"", ""Inactive"": ""allowed"", ""Active"": ""allowed"", ""Restricted"": ""allowed"", ""Blocked"": ""allowed"", ""Expired"": ""allowed"", ""Closed"": ""allowed"" }
        }
      ]
    }";
}
