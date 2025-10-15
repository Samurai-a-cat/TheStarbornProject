using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.Services.Validators;

namespace TheStarbornApp.GameCore.Services;

public static class TemplateRegistrationHelper
{
    public static IServiceCollection AddTemplateData(this IServiceCollection services)
    {
        // Регистрируем сервис загрузки шаблонов
        services.AddSingleton<ITemplateLoaderService, TemplateLoaderService>();
        services.AddSingleton<ITemplateValidator, TemplateValidator>();
        
        // Регистрируем данные шаблонов как коллекции
        services.AddSingleton<IEnumerable<AsteroidTemplateData>>(serviceProvider =>
        {
            var loader = serviceProvider.GetRequiredService<ITemplateLoaderService>();
            var validator = serviceProvider.GetRequiredService<ITemplateValidator>();
            var logger = serviceProvider.GetRequiredService<ILogger<ITemplateLoaderService>>();
            
            try
            {
                var templates = loader.LoadAsteroidTemplates().ToList();
                
                // Валидация загруженных шаблонов
                validator.ValidateTemplates(templates);

                logger.LogInformation("Registered {Count} validated asteroid templates", templates.Count);
                return templates;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load and register asteroid template data");
                throw;
            }
        });

        // Регистрируем данные шаблонов пылевых облаков
        services.AddSingleton<IEnumerable<DustCloudTemplateData>>(serviceProvider =>
        {
            var loader = serviceProvider.GetRequiredService<ITemplateLoaderService>();
            var logger = serviceProvider.GetRequiredService<ILogger<ITemplateLoaderService>>();
            
            try
            {
                var templates = loader.LoadDustCloudTemplates().ToList();
                
                logger.LogInformation("Registered {Count} dust cloud templates", templates.Count);
                return templates;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load and register dust cloud template data");
                throw;
            }
        });

        return services;
    }
}