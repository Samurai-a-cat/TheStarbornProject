using System.Reflection;
using System.Text.Json;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;

namespace TheStarbornApp.GameCore.Services;

public interface ITemplateLoaderService
{
    IEnumerable<AsteroidTemplateData> LoadAsteroidTemplates();
}

public sealed class TemplateLoaderService : ITemplateLoaderService
{
    private readonly ILogger<TemplateLoaderService> _logger;
    private readonly string _templatesDirectory;

    public TemplateLoaderService(ILogger<TemplateLoaderService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Для MAUI BLAZOR HYBRID определяем путь к ресурсам
        var basePath = GetBasePath();
        _templatesDirectory = Path.Combine(basePath, "templates");
    }

    private string GetBasePath()
    {
        // Пытаемся получить путь к ресурсам MAUI
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!string.IsNullOrEmpty(assemblyLocation))
        {
            // Проверяем наличие директории с шаблонами
            var androidPath = Path.Combine(assemblyLocation, "..", "Resources", "raw", "templates");
            var generalPath = Path.Combine(assemblyLocation, "templates");
            
            if (Directory.Exists(androidPath))
                return androidPath;
            if (Directory.Exists(generalPath))
                return generalPath;
        }
        
        // Резервный путь
        return "templates";
    }

    public IEnumerable<AsteroidTemplateData> LoadAsteroidTemplates()
    {
        var templates = new List<AsteroidTemplateData>();
        
        try
        {
            var filePath = Path.Combine(_templatesDirectory, "asteroids.json");
            
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Template file not found at: {FilePath}", filePath);
                return templates;
            }

            var jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var templateContainer = JsonSerializer.Deserialize<TemplateContainer>(jsonContent, options);
            
            if (templateContainer?.AsteroidTemplates != null)
            {
                foreach (var template in templateContainer.AsteroidTemplates)
                {
                    ValidateTemplateData(template);
                    templates.Add(template);
                }
                
                _logger.LogInformation("Successfully loaded {Count} asteroid templates from {FilePath}", 
                    templates.Count, filePath);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing JSON template file");
            throw new InvalidOperationException("Invalid JSON format in template file", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading template data from {Path}", _templatesDirectory);
            throw;
        }

        return templates;
    }
    
    public IEnumerable<DustCloudTemplateData> LoadDustCloudTemplates()
    {
        var templates = new List<DustCloudTemplateData>();
    
        try
        {
            var filePath = Path.Combine(_templatesDirectory, "dustclouds.json");
        
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Dust cloud template file not found at: {FilePath}", filePath);
                return templates;
            }

            var jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var templateContainer = JsonSerializer.Deserialize<DustCloudTemplateContainer>(jsonContent, options);
        
            if (templateContainer?.DustCloudTemplates != null)
            {
                foreach (var template in templateContainer.DustCloudTemplates)
                {
                    ValidateDustCloudTemplateData(template);
                    templates.Add(template);
                }
            
                _logger.LogInformation("Successfully loaded {Count} dust cloud templates from {FilePath}", 
                    templates.Count, filePath);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing JSON dust cloud template file");
            throw new InvalidOperationException("Invalid JSON format in dust cloud template file", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dust cloud template data from {Path}", _templatesDirectory);
            throw;
        }

        return templates;
    }
    
    private static void ValidateDustCloudTemplateData(DustCloudTemplateData template)
    {
        if (string.IsNullOrWhiteSpace(template.TemplateName))
            throw new InvalidOperationException("TemplateName cannot be null or empty");
        
        if (template.BaseDustAmount < 0)
            throw new InvalidOperationException($"BaseDustAmount cannot be negative for template {template.TemplateName}");
        
        if (template.SpawnChance < 0 || template.SpawnChance > 1)
            throw new InvalidOperationException($"SpawnChance must be between 0 and 1 for template {template.TemplateName}");
    }
    
    // Вспомогательный класс для десериализации JSON пылевых облаков
    public sealed class DustCloudTemplateContainer
    {
        public DustCloudTemplateData[]? DustCloudTemplates { get; set; }
    }

    private static void ValidateTemplateData(AsteroidTemplateData template)
    {
        if (string.IsNullOrWhiteSpace(template.TemplateName))
            throw new InvalidOperationException("TemplateName cannot be null or empty");
            
        if (template.PossibleOreTypes == null || template.PossibleOreTypes.Length == 0)
            throw new InvalidOperationException($"PossibleOreTypes cannot be null or empty for template {template.TemplateName}");
            
        if (template.BaseOreAmountMin < 0 || template.BaseOreAmountMax < 0)
            throw new InvalidOperationException($"BaseOreAmount values cannot be negative for template {template.TemplateName}");
            
        if (template.BaseOreAmountMin > template.BaseOreAmountMax)
            throw new InvalidOperationException($"BaseOreAmountMin cannot be greater than BaseOreAmountMax for template {template.TemplateName}");
    }
}

// Вспомогательный класс для десериализации JSON
public sealed class TemplateContainer
{
    public AsteroidTemplateData[]? AsteroidTemplates { get; set; }
}

