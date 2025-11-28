using AvailableActions.Business.Services.Abstraction;
using AvailableActions.Models.Models;
using AvailableActions.Models.Models.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AvailableActions.Business.Services;

public class RulesService : IRulesService
{
    private static readonly JsonSerializerOptions CachedJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly RulesModels _rules = new();
    private readonly ILogger<RulesService> _logger;

    public RulesService(string rulesFilePath, ILogger<RulesService> logger)
    {
        _logger = logger;

        try
        {
            _logger.LogInformation("Trying to load rules from: {Path}", rulesFilePath);

            if (!File.Exists(rulesFilePath))
            {
                _logger.LogWarning("Rules file not found at {Path}. Using empty rules.", rulesFilePath);
                _rules = new RulesModels();
                return;
            }

            var json = File.ReadAllText(rulesFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Rules file is empty: {Path}", rulesFilePath);
                _rules = new RulesModels();
                return;
            }

            var parsed = JsonSerializer.Deserialize<RulesModels>(json, CachedJsonOptions);

            if (parsed == null || parsed.Actions == null || parsed.Actions.Count == 0)
            {
                _logger.LogWarning("Parsed rules are empty or null. Raw JSON preview: {Preview}", json.Length > 500 ? json.Substring(0, 500) + "..." : json);
                _rules = new RulesModels();
            }
            else
            {
                _rules = parsed;
                _logger.LogInformation("Loaded {Count} action rules", _rules.Actions.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load rules");
            _rules = new RulesModels();
        }
    }

    private static RuleDecision ParseDecision(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return RuleDecision.Never;
        return raw.Trim().ToLowerInvariant() switch
        {
            "always" => RuleDecision.Always,
            "never" => RuleDecision.Never,
            "if_pin_set" => RuleDecision.IfPinSet,
            "if_pin_not_set" => RuleDecision.IfPinNotSet,
            _ => RuleDecision.Never
        };
    }

    public IEnumerable<string> GetAllowedActions(CardDetails card)
    {
        var allowed = new List<string>();

        foreach (var action in _rules.Actions)
        {
            try
            {
                action.AllowedCardTypes.TryGetValue(card.CardType.ToString(), out var typeDecisionRaw);
                var typeDecision = ParseDecision(typeDecisionRaw);
                if (typeDecision == RuleDecision.Never)
                    continue;

                action.AllowedCardStatuses.TryGetValue(card.CardStatus.ToString(), out var statusDecisionRaw);
                var statusDecision = ParseDecision(statusDecisionRaw);
                if (statusDecision == RuleDecision.Never)
                    continue;

                bool isAllowed = statusDecision switch
                {
                    RuleDecision.Always => true,
                    RuleDecision.IfPinSet => card.IsPinSet,
                    RuleDecision.IfPinNotSet => !card.IsPinSet,
                    _ => false
                };

                if (typeDecision == RuleDecision.IfPinSet && !card.IsPinSet) 
                    isAllowed = false;
                if (typeDecision == RuleDecision.IfPinNotSet && card.IsPinSet) 
                    isAllowed = false;

                if (isAllowed)
                    allowed.Add(action.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while evaluating rule {Action}. Skipping.", action.Name);
            }
        }

        return allowed;
    }
}
