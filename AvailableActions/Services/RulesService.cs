using AvailableActions.Models;
using AvailableActions.Models.Enums;
using AvailableActions.Services.Abstraction;
using System.Text.Json;

public class RulesService : IRulesService
{
    private RulesModels _rules = new();
    private readonly ILogger<RulesService> _logger;
    private readonly IConfiguration _configuration;

    public RulesService(IConfiguration configuration, ILogger<RulesService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        try
        {
            var rulesFilePath = _configuration["Rules:Path"]
                                ?? Path.Combine(AppContext.BaseDirectory, "rules.json");

            _logger.LogInformation("Loading rules from {Path}", rulesFilePath);

            if (!File.Exists(rulesFilePath))
            {
                _logger.LogWarning("Rules file not found at {Path}. Using empty rules set.", rulesFilePath);
                _rules = new RulesModels();
            }
            else
            {
                var json = File.ReadAllText(rulesFilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var parsed = JsonSerializer.Deserialize<RulesModels>(json, options);
                _rules = parsed ?? new RulesModels();
            }

            _logger.LogInformation("Loaded {Count} action rules", _rules.Actions?.Count ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load rules file: {Message}", ex.Message);
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

                if (typeDecision == RuleDecision.IfPinSet && !card.IsPinSet) isAllowed = false;
                if (typeDecision == RuleDecision.IfPinNotSet && card.IsPinSet) isAllowed = false;

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
   
    