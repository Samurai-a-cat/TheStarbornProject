// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/Asteroids/AsteroidBlueprint.cs
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

public sealed class AsteroidBlueprint : IBlueprint<AsteroidInstance>
{
    public string BlueprintName { get; }
    public double BaseMass { get; }
    public double BaseDensity { get; }
    public string[] PossibleOreTypes { get; }
    public int BaseOreAmountMin { get; }
    public int BaseOreAmountMax { get; }
    public double SpawnProbabilityFactor { get; }

    public AsteroidBlueprint(
        string templateName,
        double baseMass,
        double baseDensity,
        string[] possibleOreTypes,
        int baseOreAmountMin,
        int baseOreAmountMax,
        double spawnProbabilityFactor)
    {
        BlueprintName = templateName ?? throw new ArgumentNullException(nameof(templateName));
        BaseMass = baseMass;
        BaseDensity = baseDensity;
        PossibleOreTypes = possibleOreTypes ?? throw new ArgumentNullException(nameof(possibleOreTypes));
        if (possibleOreTypes.Length == 0)
            throw new ArgumentException("PossibleOreTypes cannot be empty.", nameof(possibleOreTypes));
        BaseOreAmountMin = baseOreAmountMin;
        BaseOreAmountMax = baseOreAmountMax;
        SpawnProbabilityFactor = spawnProbabilityFactor;
    }

    // --- Логика генерации на основе контекста и RNG ---
    private string GetOreTypeFromContext(SharedContext context, Random rng)
    {
        return PossibleOreTypes[rng.Next(PossibleOreTypes.Length)];
    }

    private int GetBaseOreAmount(Random rng)
    {
        var range = BaseOreAmountMax - BaseOreAmountMin;
        return BaseOreAmountMin + rng.Next(range + 1);
    }

    private bool ShouldSpawn(SharedContext context, Random rng)
    {
        double fakeAsteroidFrequency = 0.5; 
        double effectiveSpawnChance = BaseDensity * fakeAsteroidFrequency * SpawnProbabilityFactor;
        return rng.NextDouble() < effectiveSpawnChance;
    }

    private double GetResourceMultiplier(SharedContext context)
    {
        return 1.0; // Заглушка
    }

    // --- Реализация IBlueprint<AsteroidInstance>.CreateCustomEntity ---
    public AsteroidInstance? CreateCustomEntity(EntityId id, SharedContext context, Random rng)
    {
        if (!ShouldSpawn(context, rng))
        {
            return null;
        }

        // --- Вычисляем параметры инстанса на основе шаблона, контекста и RNG ---
        string oreType = GetOreTypeFromContext(context, rng);
        int baseOreAmount = GetBaseOreAmount(rng);
        double resourceMultiplier = GetResourceMultiplier(context);
        int finalOreAmount = (int)(baseOreAmount * resourceMultiplier);

        if (finalOreAmount <= 0) finalOreAmount = 1; // Минимум 1 единица руды

        // Создаём инстанс
        return new AsteroidInstance(id, oreType, finalOreAmount, BaseMass, BaseDensity);
    }
}