using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;

public interface IDustCloudFactory
{
    DustCloudInstance CreateDustCloudInstance(
        DustCloudTemplateData templateData,
        EntityId instanceId,
        SectorId sector,
        long gameSeconds,
        SharedContext context,
        IEnumerable<Intervention> interventions,
        Random rng);
}

public class DustCloudFactory : IDustCloudFactory
{
    public DustCloudInstance CreateDustCloudInstance(
        DustCloudTemplateData templateData,
        EntityId instanceId,
        SectorId sector,
        long gameSeconds,
        SharedContext context,
        IEnumerable<Intervention> interventions,
        Random rng)
    {
        // Создаём шаблон из данных (выводим)
        var template = new DustCloudTemplate(
            templateData.TemplateName,
            templateData.BaseDustAmount,
            templateData.ValuableResourceMultiplier,
            templateData.SpawnChance,
            templateData.OptimalDistance,
            templateData.DistanceInfluenceFactor);

        // Используем шаблон и контекст для вычисления состояния инстанса
        var baseDustAmount = template.CalculateDustAmount(sector, context, rng);
        var valuableAmount = template.CalculateValuableResourceAmount(baseDustAmount, sector, context, rng);

        // Создаём инстанс с вычисленными данными
        return new DustCloudInstance(instanceId, baseDustAmount, valuableAmount);
    }
}