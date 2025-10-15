using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

namespace TheStarbornApp.GameCore.Services.Validators;

public interface ITemplateValidator
{
    void ValidateTemplates(IEnumerable<AsteroidTemplateData> templates);
}

public sealed class TemplateValidator : ITemplateValidator
{
    private readonly ILogger<TemplateValidator> _logger;

    public TemplateValidator(ILogger<TemplateValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ValidateTemplates(IEnumerable<AsteroidTemplateData> templates)
    {
        var templateList = templates.ToList();
        
        // Проверка уникальности имён шаблонов
        var duplicateNames = templateList
            .GroupBy(t => t.TemplateName, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Any())
        {
            var errorMsg = $"Duplicate template names found: {string.Join(", ", duplicateNames)}";
            _logger.LogError(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }

        // Проверка корректности значений для каждого шаблона
        foreach (var template in templateList)
        {
            ValidateSingleTemplate(template);
        }

        _logger.LogInformation("Successfully validated {Count} templates", templateList.Count);
    }

    private void ValidateSingleTemplate(AsteroidTemplateData template)
    {
        if (string.IsNullOrWhiteSpace(template.TemplateName))
            throw new InvalidOperationException("TemplateName cannot be null or empty");
            
        if (template.PossibleOreTypes == null || template.PossibleOreTypes.Length == 0)
            throw new InvalidOperationException($"PossibleOreTypes cannot be null or empty for template {template.TemplateName}");
            
        if (template.BaseOreAmountMin < 0 || template.BaseOreAmountMax < 0)
            throw new InvalidOperationException($"BaseOreAmount values cannot be negative for template {template.TemplateName}");
            
        if (template.BaseOreAmountMin > template.BaseOreAmountMax)
            throw new InvalidOperationException($"BaseOreAmountMin cannot be greater than BaseOreAmountMax for template {template.TemplateName}");
            
        if (template.BaseMass <= 0)
            throw new InvalidOperationException($"BaseMass must be positive for template {template.TemplateName}");
            
        if (template.BaseDensity < 0 || template.BaseDensity > 1)
            throw new InvalidOperationException($"BaseDensity must be between 0 and 1 for template {template.TemplateName}");
    }
}