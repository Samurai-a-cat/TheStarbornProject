// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/Asteroids/AsteroidTemplate.cs

using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

public sealed class AsteroidTemplate
{
    public string TemplateName { get; }
    public double BaseMass { get; }
    public double BaseScanDifficulty { get; }
    public string[] PossibleOreTypes { get; }
    public double BaseOreAmountMin { get; }
    public double BaseOreAmountMax { get; }
    public double BaseDensity { get; }

    public AsteroidTemplate(
        string templateName,
        double baseMass,
        double baseScanDifficulty,
        string[] possibleOreTypes,
        double baseOreAmountMin,
        double baseOreAmountMax,
        double baseDensity)
    {
        TemplateName = templateName ?? throw new ArgumentNullException(nameof(templateName));
        BaseMass = baseMass;
        BaseScanDifficulty = baseScanDifficulty;
        PossibleOreTypes = possibleOreTypes ?? throw new ArgumentNullException(nameof(possibleOreTypes));
        if (possibleOreTypes.Length == 0)
            throw new ArgumentException("PossibleOreTypes cannot be empty.", nameof(possibleOreTypes));
        BaseOreAmountMin = baseOreAmountMin;
        BaseOreAmountMax = baseOreAmountMax;
        BaseDensity = baseDensity;
    }

    // --- Логика генерации на основе контекста и RNG ---
    public string GetOreTypeFromContext(SharedContext context, Random rng)
    {
        return PossibleOreTypes[rng.Next(PossibleOreTypes.Length)];
    }

    public int GetBaseOreAmount(Random rng)
    {
        var range = BaseOreAmountMax - BaseOreAmountMin;
        return (int)(BaseOreAmountMin + rng.NextDouble() * range);
    }

    public bool ShouldSpawn(SharedContext context, Random rng)
    {
        return rng.NextDouble() < BaseDensity * context.AsteroidDensity;
    }

    public double GetResourceMultiplier(SharedContext context)
    {
        return context.ResourceAbundance;
    }
}