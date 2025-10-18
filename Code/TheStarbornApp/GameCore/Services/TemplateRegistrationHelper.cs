using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.GasClouds;
using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.Services.Validators;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

namespace TheStarbornApp.GameCore.Services;

public static class TemplateRegistrationHelper
{
    public static IServiceCollection AddTemplateData(this IServiceCollection services)
    {
        // Регистрируем сервис загрузки шаблонов
        services.AddSingleton<ITemplateLoaderService, TemplateLoaderService>();
        // ITemplateValidator больше не нужен и не регистрируется

        // Регистрируем данные шаблонов как коллекции

        // --- Астероиды ---
        services.AddSingleton<IEnumerable<AsteroidTemplateData>>(serviceProvider =>
        {
            var loader = serviceProvider.GetRequiredService<ITemplateLoaderService>();
            var logger = serviceProvider.GetRequiredService<ILogger<ITemplateLoaderService>>();

            try
            {
                var templates = loader.LoadAsteroidTemplates().ToList();

                // Валидация загруженных шаблонов через IValidatableTemplateData
                // Предполагается, что AsteroidTemplateData реализует IValidatableTemplateData<AsteroidTemplateData>
                // и благодаря ковариантности 'in T' может быть приведен к IValidatableTemplateData<ITemplateData>
                ValidateTemplates(templates.OfType<IValidatableTemplateData<ITemplateData>>(), logger);

                logger.LogInformation("Registered {Count} validated asteroid templates", templates.Count);
                return templates;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load and register asteroid template data");
                throw;
            }
        });

        // --- Газовые облака ---
        services.AddSingleton<IEnumerable<GasCloudTemplateData>>(serviceProvider =>
        {
            var loader = serviceProvider.GetRequiredService<ITemplateLoaderService>();
            var logger = serviceProvider.GetRequiredService<ILogger<ITemplateLoaderService>>();

            try
            {
                var templates = loader.LoadGasCloudTemplates().ToList();

                // Валидация загруженных шаблонов через IValidatableTemplateData
                // Предполагается, что GasCloudTemplateData реализует IValidatableTemplateData<GasCloudTemplateData>
                // и благодаря ковариантности 'in T' может быть приведен к IValidatableTemplateData<ITemplateData>
                ValidateTemplates(templates.OfType<IValidatableTemplateData<ITemplateData>>(), logger);

                logger.LogInformation("Registered {Count} validated gas cloud templates", templates.Count);
                return templates;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load and register gas cloud template data");
                throw;
            }
        });

        // --- Обобщённая коллекция ITemplateData ---
        // Это ключевой момент: TemplateRegistry принимает IEnumerable<ITemplateData>
        services.AddSingleton<IEnumerable<ITemplateData>>(serviceProvider =>
        {
            var asteroidTemplates = serviceProvider.GetRequiredService<IEnumerable<AsteroidTemplateData>>().ToList();
            var gasCloudTemplates = serviceProvider.GetRequiredService<IEnumerable<GasCloudTemplateData>>().ToList();

            // Объединяем все коллекции ITemplateData
            var allTemplates = new List<ITemplateData>();
            allTemplates.AddRange(asteroidTemplates);
            allTemplates.AddRange(gasCloudTemplates);

            // Проверка уникальности имён для всех шаблонов (теперь делается *после* валидации специфичных шаблонов)
            // Это логично, так как валидация уникальности имён может быть частью общей валидации ITemplateData,
            // но в текущем дизайне она отдельная.
            ValidateTemplateNamesUniqueness(allTemplates);

            return allTemplates;
        });

        return services;
    }

    /// <summary>
    /// Валидирует коллекцию шаблонов, реализующих IValidatableTemplateData.
    /// </summary>
    /// <param name="validatableTemplates">Коллекция шаблонов для валидации.</param>
    /// <param name="logger">Логгер для сообщений.</param>
    private static void ValidateTemplates(IEnumerable<IValidatableTemplateData<ITemplateData>> validatableTemplates, ILogger logger)
    {
        var templatesList = validatableTemplates.ToList(); // Избегаем многократной перечисляемости
        foreach (var template in templatesList)
        {
            template.Validate(); // Вызывает общую и специфичную валидацию
        }
        logger.LogInformation("Successfully validated {Count} templates using IValidatableTemplateData", templatesList.Count);
    }

    /// <summary>
    /// Проверяет уникальность имён шаблонов в коллекции.
    /// </summary>
    /// <param name="templates">Коллекция шаблонов.</param>
    private static void ValidateTemplateNamesUniqueness(IEnumerable<ITemplateData> templates)
    {
        var templateList = templates.ToList(); // Избегаем многократной перечисляемости
        var duplicateNames = templateList
            .GroupBy(t => t.TemplateName, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (!duplicateNames.Any()) return;
        var errorMsg = $"Duplicate template names found: {string.Join(", ", duplicateNames)}";
        throw new InvalidOperationException(errorMsg);
    }
}