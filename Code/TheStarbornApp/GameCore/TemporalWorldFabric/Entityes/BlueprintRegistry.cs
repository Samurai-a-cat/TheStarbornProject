using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.GasClouds;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public interface IBlueprintRegistry
{
    // Получить Blueprint по имени
    IBlueprint<T>? GetBlueprint<T>(string blueprintName) where T : class, IEntity;
    // Получить все Blueprint-ы определенного типа
    // IReadOnlyList<IBlueprint<T>> GetAllBlueprints<T>() where T : IEntity;
    // Получить все имена
    // IReadOnlyCollection<string> GetAllBlueprintNames();
}

public sealed class BlueprintRegistry : IBlueprintRegistry
{
    private readonly ILogger<BlueprintRegistry>? _logger;
    // Словарь для хранения IBlueprint<T> под интерфейсом, с приведением типов при необходимости
    private readonly Dictionary<string, IBlueprint<IEntity>> _blueprints;

    public BlueprintRegistry(
        IEnumerable<IBlueprint<IEntity>> blueprints, // Или IEnumerable<IBlueprint<AsteroidInstance>>, IEnumerable<IBlueprint<GasCloudInstance>> и т.д. если раздельно
        ILogger<BlueprintRegistry>? logger = null)
    {
        _logger = logger;
        _blueprints = new Dictionary<string, IBlueprint<IEntity>>(StringComparer.OrdinalIgnoreCase);

        foreach (var blueprint in blueprints)
        {
            // Предположим, у IBlueprint<T> есть свойство Name или метод GetName()
            // Для этого нужно будет добавить свойство Name в IBlueprint<T>
            var name = GetBlueprintName(blueprint); // Вспомогательный метод или свойство в IBlueprint
            if (_blueprints.ContainsKey(name))
            {
                _logger?.LogError("Duplicate blueprint name '{BlueprintName}' found.", name);
                throw new InvalidOperationException($"Duplicate blueprint name '{name}'.");
            }
            _blueprints[name] = blueprint;
        }

        _logger?.LogInformation("Successfully registered {Count} blueprint entries.", _blueprints.Count);
    }

    // Вспомогательный метод (или свойство Name в IBlueprint)
    // Для упрощения примера, предположим, что у Blueprint есть свойство TemplateName (как в AsteroidBlueprint)
    private string GetBlueprintName(IBlueprint<IEntity> blueprint)
    {
        // Это хак, если IBlueprint<IEntity> не знает своего внутреннего типа T.
        // Лучше добавить Name в IBlueprint<T>
        if (blueprint is AsteroidBlueprint asteroidBp) return asteroidBp.BlueprintName;
        if (blueprint is GasCloudBlueprint gasBp) return gasBp.TemplateName;
        // ... другие кейсы ...
        //throw new InvalidOperationException($"Unknown blueprint type: {blueprint.GetType().Name}");
        // Лучше добавить Name в IBlueprint<T>
        return "unknown"; // Временно
    }

    public TBlueprint? GetBlueprint<T>(string blueprintName) where T : IEntity
    {
        if (_blueprints.TryGetValue(blueprintName, out var genericBlueprint))
        {
            // Проверяем, совместим ли тип
            if (genericBlueprint is IBlueprint<T> specificBlueprint)
            {
                _logger?.LogDebug("Found blueprint of type {BlueprintType} with name {BlueprintName}", typeof(T).Name, blueprintName);
                return specificBlueprint;
            }
            else
            {
                _logger?.LogWarning("Blueprint with name {BlueprintName} exists but is not of type {EntityType}.", blueprintName, typeof(T).Name);
                return null;
            }
        }
        _logger?.LogWarning("Blueprint with name {BlueprintName} of type {EntityType} was not found.", blueprintName, typeof(T).Name);
        return null;
    }
    // ... другие методы ...
}