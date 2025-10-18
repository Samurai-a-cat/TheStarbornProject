// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/GasClouds/GasCloudBlueprint.cs
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.GasClouds;

public sealed class GasCloudBlueprint : IBlueprint<GasCloudInstance>
{
    public string TemplateName { get; }
    public double BaseDensity { get; }
    public string[] PossibleGasTypes { get; }
    public int BaseGasAmountMin { get; }
    public int BaseGasAmountMax { get; }
    public double SpawnProbabilityFactor { get; }

    public GasCloudBlueprint(
        string templateName,
        double baseDensity,
        string[] possibleGasTypes,
        int baseGasAmountMin,
        int baseGasAmountMax,
        double spawnProbabilityFactor)
    {
        TemplateName = templateName;
        BaseDensity = baseDensity;
        PossibleGasTypes = possibleGasTypes;
        BaseGasAmountMin = baseGasAmountMin;
        BaseGasAmountMax = baseGasAmountMax;
        SpawnProbabilityFactor = spawnProbabilityFactor;
    }

    // --- Логика генерации на основе контекста и RNG ---
    // (Перенесены из старого GasCloudTemplate.cs)
    private string GetGasTypeFromContext(SharedContext context, Random rng)
    {
        return PossibleGasTypes[rng.Next(PossibleGasTypes.Length)];
    }

    private int GetBaseGasAmount(Random rng)
    {
        var range = BaseGasAmountMax - BaseGasAmountMin;
        return BaseGasAmountMin + rng.Next(range + 1);
    }

    private bool ShouldSpawn(SharedContext context, Random rng)
    {
        double fakeGasCloudFrequency = 0.5; // Заменить на `context.GasCloudFrequency` позже
        double effectiveSpawnChance = BaseDensity * fakeGasCloudFrequency * SpawnProbabilityFactor;
        return rng.NextDouble() < effectiveSpawnChance;
    }

    private double GetGasMultiplier(SharedContext context)
    {
        return 1.0; // Заглушка
    }

    // --- Реализация IBlueprint<GasCloudInstance>.CreateCustomEntity ---
    public GasCloudInstance? CreateCustomEntity(EntityId id, SharedContext context, Random rng)
    {
        if (!ShouldSpawn(context, rng))
        {
            return null; // Не спавнится в этом секторе/контексте
        }

        // --- Вычисляем параметры инстанса на основе шаблона, контекста и RNG ---
        string gasType = GetGasTypeFromContext(context, rng);
        int baseGasAmount = GetBaseGasAmount(rng);
        double gasMultiplier = GetGasMultiplier(context);
        int finalGasAmount = (int)(baseGasAmount * gasMultiplier);

        if (finalGasAmount <= 0) finalGasAmount = 1; // Минимум 1 единица газа

        // Создаём инстанс
        return new GasCloudInstance(id, gasType, finalGasAmount, BaseDensity);
    }
}