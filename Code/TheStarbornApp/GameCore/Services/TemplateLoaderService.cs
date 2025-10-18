// File: TheStarbornApp/GameCore/Services/TemplateLoaderService.cs
using System.Reflection;
using System.Text.Json;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.GasClouds;
using Microsoft.Extensions.Logging;

namespace TheStarbornApp.GameCore.Services;

// --- Обновлённый интерфейс ---
public interface ITemplateLoaderService
{
    IEnumerable<AsteroidTemplateData> LoadAsteroidTemplates();
    IEnumerable<GasCloudTemplateData> LoadGasCloudTemplates();
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

            var templateContainer = JsonSerializer.Deserialize<AsteroidTemplateContainer>(jsonContent, options); // Обновлено
            
            if (templateContainer?.AsteroidTemplates != null)
            {
                foreach (var template in templateContainer.AsteroidTemplates)
                {
                    ValidateAsteroidTemplateData(template);
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
    
    // --- Новый метод для газовых облаков ---
    public IEnumerable<GasCloudTemplateData> LoadGasCloudTemplates()
    {
        var templates = new List<GasCloudTemplateData>();
    
        try
        {
            var filePath = Path.Combine(_templatesDirectory, "gas_clouds.json"); // Имя файла из предыдущего шага
        
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Gas cloud template file not found at: {FilePath}", filePath);
                return templates;
            }

            var jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var templateContainer = JsonSerializer.Deserialize<GasCloudTemplateContainer>(jsonContent, options); // Обновлено
        
            if (templateContainer?.GasCloudTemplates != null)
            {
                foreach (var template in templateContainer.GasCloudTemplates)
                {
                    ValidateGasCloudTemplateData(template); // Обновлено
                    templates.Add(template);
                }
            
                _logger.LogInformation("Successfully loaded {Count} gas cloud templates from {FilePath}", 
                    templates.Count, filePath);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing JSON gas cloud template file");
            throw new InvalidOperationException("Invalid JSON format in gas cloud template file", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gas cloud template data from {Path}", _templatesDirectory);
            throw;
        }

        return templates;
    }
    
    // --- Валидация для газовых облаков ---
    private static void ValidateGasCloudTemplateData(GasCloudTemplateData template)
    {
        if (string.IsNullOrWhiteSpace(template.TemplateName))
            throw new InvalidOperationException("TemplateName for GasCloud cannot be null or empty");
        
        if (template.PossibleGasTypes == null || template.PossibleGasTypes.Length == 0)
            throw new InvalidOperationException($"PossibleGasTypes for GasCloud cannot be null or empty for template {template.TemplateName}");
            
        if (template.BaseGasAmountMin < 0 || template.BaseGasAmountMax < 0)
            throw new InvalidOperationException($"BaseGasAmount values for GasCloud cannot be negative for template {template.TemplateName}");
            
        if (template.BaseGasAmountMin > template.BaseGasAmountMax)
            throw new InvalidOperationException($"BaseGasAmountMin cannot be greater than BaseGasAmountMax for GasCloud template {template.TemplateName}");

        if (template.BaseDensity < 0)
            throw new InvalidOperationException($"BaseDensity for GasCloud cannot be negative for template {template.TemplateName}");

        if (template.SpawnProbabilityFactor < 0)
            throw new InvalidOperationException($"SpawnProbabilityFactor for GasCloud cannot be negative for template {template.TemplateName}");
    }
    
    // --- Валидация для астероидов (оставлена как есть) ---
    private static void ValidateAsteroidTemplateData(AsteroidTemplateData template)
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

    // --- Вспомогательные классы для десериализации (внутри класса) ---
    private sealed class AsteroidTemplateContainer
    {
        public AsteroidTemplateData[]? AsteroidTemplates { get; set; }
    }

    private sealed class GasCloudTemplateContainer
    {
        public GasCloudTemplateData[]? GasCloudTemplates { get; set; }
    }
}