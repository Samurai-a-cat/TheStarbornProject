using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;
using System.Collections.ObjectModel;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public interface ISectorPopulator
{
    IReadOnlyList<IEntity> PopulateSector(SectorId sector, long gameSeconds, ulong worldSeed);
}

public class SectorPopulator : ISectorPopulator
{
    private readonly ITemplateRegistry _templateRegistry;
    private readonly IAsteroidFactory _asteroidFactory;
    private readonly IDustCloudFactory _dustCloudFactory;
    private readonly ISectorContextGenerator _contextGenerator;

    public SectorPopulator(
        ITemplateRegistry templateRegistry,
        IAsteroidFactory asteroidFactory,
        IDustCloudFactory dustCloudFactory,
        ISectorContextGenerator contextGenerator)
    {
        _templateRegistry = templateRegistry ?? throw new ArgumentNullException(nameof(templateRegistry));
        _asteroidFactory = asteroidFactory ?? throw new ArgumentNullException(nameof(asteroidFactory));
        _dustCloudFactory = dustCloudFactory ?? throw new ArgumentNullException(nameof(dustCloudFactory));
        _contextGenerator = contextGenerator ?? throw new ArgumentNullException(nameof(contextGenerator));
    }

    public IReadOnlyList<IEntity> PopulateSector(SectorId sector, long gameSeconds, ulong worldSeed)
    {
        var rng = new Random((int)HashCode.Combine(sector.X, sector.Y, sector.Z, worldSeed));
        var context = _contextGenerator.Generate(sector, gameSeconds, worldSeed);

        var entities = new List<IEntity>();

        // --- Астероиды ---
        var asteroidTemplateData = _templateRegistry.GetData<AsteroidTemplateData>("enriched-asteroid");
        if (asteroidTemplateData != null)
        {
            // Создаём шаблон из данных для проверки спавна
            var asteroidTemplate = new AsteroidTemplate(
                asteroidTemplateData.TemplateName,
                asteroidTemplateData.BaseMass,
                asteroidTemplateData.BaseScanDifficulty,
                asteroidTemplateData.PossibleOreTypes,
                asteroidTemplateData.BaseOreAmountMin,
                asteroidTemplateData.BaseOreAmountMax,
                asteroidTemplateData.BaseDensity);

            for (int i = 0; i < 5; i++)
            {
                if (asteroidTemplate.ShouldSpawn(context, rng))
                {
                    var entityId = GenerateEntityId(sector, "enriched-asteroid", i);
                    var instance = _asteroidFactory.CreateAsteroidInstance(
                        asteroidTemplateData, entityId, sector, gameSeconds, context, Enumerable.Empty<Intervention>(), rng);
                    entities.Add(instance);
                }
            }
        }

        // --- Пылевые облака ---
        // Пока оставляем как есть, будет обновлено в будущем
        // var dustCloudTemplateData = _templateRegistry.GetData<DustCloudTemplateData>("dust-cloud");
        // if (dustCloudTemplateData != null)
        // {
        //     // Аналогично для пылевых облаков
        // }

        return new ReadOnlyCollection<IEntity>(entities);
    }

    private EntityId GenerateEntityId(SectorId sector, string type, int index)
    {
        return new EntityId($"{type}-{sector.X}_{sector.Y}_{sector.Z}-{index}");
    }
}