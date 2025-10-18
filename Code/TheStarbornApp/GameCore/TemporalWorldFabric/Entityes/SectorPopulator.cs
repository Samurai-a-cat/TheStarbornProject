using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;
using System.Collections.ObjectModel;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.GasClouds;
namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public interface ISectorPopulator
{
    IReadOnlyList<IEntity> PopulateSector(SectorId sector, long gameSeconds, ulong worldSeed);
}

public class SectorPopulator : ISectorPopulator
{
    private readonly ITemplateRegistry _templateRegistry; // Оставляем, если газовые облака всё ещё используют TemplateData
    private readonly ISectorContextGenerator _contextGenerator;
    private readonly AsteroidBlueprint _asteroidBlueprint; // Добавляем новый параметр

    public SectorPopulator(
        // IAsteroidFactory asteroidFactory, // Убираем из параметров
        ITemplateRegistry templateRegistry, // Оставляем, если газовые облака всё ещё используют TemplateData
        ISectorContextGenerator contextGenerator,
        AsteroidBlueprint asteroidBlueprint) // Добавляем новый параметр
    {
        _templateRegistry = templateRegistry ?? throw new ArgumentNullException(nameof(templateRegistry));
        _contextGenerator = contextGenerator ?? throw new ArgumentNullException(nameof(contextGenerator));
        _asteroidBlueprint = asteroidBlueprint ?? throw new ArgumentNullException(nameof(asteroidBlueprint));
    }

    public IReadOnlyList<IEntity> PopulateSector(SectorId sector, long gameSeconds, ulong worldSeed)
    {
        var rng = new Random((int)HashCode.Combine(sector.X, sector.Y, sector.Z, worldSeed));
        var context = _contextGenerator.Generate(sector, gameSeconds, worldSeed);
        var entities = new List<IEntity>();

        // --- Астероиды (НОВАЯ ЛОГИКА) ---
        for (int i = 0; i < 5; i++) // Количество попыток спавна
        {
            var entityId = GenerateEntityId(sector, "asteroid", i); // Изменим префикс
            // Вызываем CreateCustomEntity у готового Blueprint
            var asteroidInstance = _asteroidBlueprint.CreateCustomEntity(entityId, context, rng);
            if (asteroidInstance != null) // CreateCustomEntity может вернуть null, если не должен спавниться
            {
                entities.Add(asteroidInstance);
            }
        }

        return new ReadOnlyCollection<IEntity>(entities);
    }

    private EntityId GenerateEntityId(SectorId sector, string type, int index)
    {
        return new EntityId($"{type}-{sector.X}_{sector.Y}_{sector.Z}-{index}");
    }
}