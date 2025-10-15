using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;

public sealed class DustCloudTemplate
{
    public string TemplateName { get; }
    public double BaseDustAmount { get; }
    public double ValuableResourceMultiplier { get; }
    public double SpawnChance { get; }
    public double OptimalDistance { get; }
    public double DistanceInfluenceFactor { get; }

    public DustCloudTemplate(
        string templateName,
        double baseDustAmount,
        double valuableResourceMultiplier,
        double spawnChance,
        double optimalDistance,
        double distanceInfluenceFactor)
    {
        TemplateName = templateName ?? throw new ArgumentNullException(nameof(templateName));
        BaseDustAmount = baseDustAmount;
        ValuableResourceMultiplier = valuableResourceMultiplier;
        SpawnChance = spawnChance;
        OptimalDistance = optimalDistance;
        DistanceInfluenceFactor = distanceInfluenceFactor;
    }

    // --- Логика генерации на основе контекста и RNG ---
    public int CalculateDustAmount(SectorId sector, SharedContext context, Random rng)
    {
        var distanceFromCenter = Math.Sqrt(sector.X * sector.X + sector.Y * sector.Y + sector.Z * sector.Z);
        var distanceFactor = 1.0 - Math.Abs(distanceFromCenter - OptimalDistance) / OptimalDistance;
        return (int)(BaseDustAmount * Math.Clamp(distanceFactor, 0.0, 1.0));
    }

    public int CalculateValuableResourceAmount(int baseDustAmount, SectorId sector, SharedContext context, Random rng)
    {
        var distanceFromCenter = Math.Sqrt(sector.X * sector.X + sector.Y * sector.Y + sector.Z * sector.Z);
        var valueFactor = Math.Clamp(1.0 - (distanceFromCenter / (200.0 * DistanceInfluenceFactor)), 0.0, 1.0);
        return (int)(baseDustAmount * valueFactor * ValuableResourceMultiplier);
    }

    public bool ShouldSpawn(SharedContext context, Random rng)
    {
        var contextModifier = context.DustCloudSpawnModifier;
        return rng.NextDouble() < SpawnChance * contextModifier;
    }
}