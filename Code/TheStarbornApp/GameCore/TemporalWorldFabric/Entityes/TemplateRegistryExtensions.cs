using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

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
}