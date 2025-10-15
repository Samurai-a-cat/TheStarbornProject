using System.Reflection;
using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public interface ITemplateRegistry
{
    TData? GetData<TData>(string templateName) where TData : class, ITemplateData;
    ITemplateData? GetData(string templateName);
    IReadOnlyCollection<string> GetAllTemplateNames();
}

public sealed class TemplateRegistry : ITemplateRegistry
{
    private readonly ILogger<TemplateRegistry>? _logger;
    private readonly Dictionary<string, ITemplateData> _templateData;

    public TemplateRegistry(
        IEnumerable<ITemplateData> templateData, 
        ILogger<TemplateRegistry>? logger = null) : this(templateData.ToList(), logger)
    {
    }

    private TemplateRegistry(
        List<ITemplateData> templateData, 
        ILogger<TemplateRegistry>? logger = null)
    {
        _logger = logger;
        _templateData = new Dictionary<string, ITemplateData>(StringComparer.OrdinalIgnoreCase);

        foreach (var data in templateData)
        {
            if (_templateData.ContainsKey(data.TemplateName))
            {
                _logger?.LogError("Duplicate template name '{TemplateName}' found.", data.TemplateName);
                throw new InvalidOperationException($"Duplicate template name '{data.TemplateName}'.");
            }

            _templateData[data.TemplateName] = data;
        }

        _logger?.LogInformation("Successfully registered {Count} template data entries.", _templateData.Count);
    }

    public TData? GetData<TData>(string templateName) where TData : class, ITemplateData
    {
        if (_templateData.TryGetValue(templateName, out var data) && data is TData specificData)
        {
            _logger?.LogDebug("Found template data of type {TemplateType} with name {TemplateName}", typeof(TData).Name, templateName);
            return specificData;
        }

        _logger?.LogWarning("Template data with name {TemplateName} of type {TemplateType} was not found.", templateName, typeof(TData).Name);
        return null;
    }

    public ITemplateData? GetData(string templateName)
    {
        if (_templateData.TryGetValue(templateName, out var data))
        {
            _logger?.LogDebug("Found template data with name {TemplateName}", templateName);
            return data;
        }

        _logger?.LogWarning("Template data with name {TemplateName} was not found.", templateName);
        return null;
    }

    public IReadOnlyCollection<string> GetAllTemplateNames()
    {
        _logger?.LogDebug("Returning list of all template data names.");
        return _templateData.Keys.ToList().AsReadOnly();
    }
}