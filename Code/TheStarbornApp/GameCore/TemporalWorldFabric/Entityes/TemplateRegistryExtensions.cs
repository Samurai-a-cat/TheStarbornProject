using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.GasClouds;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public static class TemplateRegistryExtensions
{
    public static AsteroidTemplate? GetAsteroidTemplate(this ITemplateRegistry registry, string templateName)
    {
        var templateData = registry.GetData<AsteroidTemplateData>(templateName);
        if (templateData == null)
            return null;

        return new AsteroidTemplate(
            templateData.TemplateName,
            templateData.BaseMass,
            templateData.BaseScanDifficulty,
            templateData.PossibleOreTypes,
            templateData.BaseOreAmountMin,
            templateData.BaseOreAmountMax,
            templateData.BaseDensity);
    }
    
    // --- Новый метод для газовых облаков ---
    public static GasCloudBlueprint? GetGasCloudTemplate(this ITemplateRegistry registry, string templateName)
    {
        var templateData = registry.GetData<GasCloudTemplateData>(templateName);
        if (templateData == null)
            return null;

        return new GasCloudBlueprint(
            templateData.TemplateName,
            templateData.BaseDensity,
            templateData.PossibleGasTypes,
            templateData.BaseGasAmountMin,
            templateData.BaseGasAmountMax,
            templateData.SpawnProbabilityFactor);
    }
}