// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/Asteroids/AsteroidFactory.cs

using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

public interface IAsteroidFactory
{
    EnrichedAsteroidInstance CreateAsteroidInstance(
        AsteroidTemplateData templateData,
        EntityId instanceId,
        SectorId sector,
        long gameSeconds,
        SharedContext context,
        IEnumerable<Intervention> interventions,
        Random rng);
}

public class AsteroidFactory : IAsteroidFactory
{
    public EnrichedAsteroidInstance CreateAsteroidInstance(
        AsteroidTemplateData templateData,
        EntityId instanceId,
        SectorId sector,
        long gameSeconds,
        SharedContext context,
        IEnumerable<Intervention> interventions,
        Random rng)
    {
        // Создаём шаблон из данных (выводим)
        var template = new AsteroidTemplate(
            templateData.TemplateName,
            templateData.BaseMass,
            templateData.BaseScanDifficulty,
            templateData.PossibleOreTypes,
            templateData.BaseOreAmountMin,
            templateData.BaseOreAmountMax,
            templateData.BaseDensity);

        // Используем шаблон и контекст для вычисления состояния инстанса
        var oreType = template.GetOreTypeFromContext(context, rng);
        var baseAmount = template.GetBaseOreAmount(rng);
        var resourceMultiplier = template.GetResourceMultiplier(context);
        var oreAmount = (int)(baseAmount * resourceMultiplier);

        // Создаём инстанс с вычисленными данными
        return new EnrichedAsteroidInstance(instanceId, oreType, oreAmount, template.BaseMass);
    }
}