using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public interface ISectorPopulator
{
    IReadOnlyList<IEntity> Populate(
        SectorId sector,
        long gameSeconds,
        ulong worldSeed,
        IEnumerable<Intervention> interventions);
}

public class SectorPopulator(PrototypeRegistry registry, ISectorContextGenerator contextGen)
    : ISectorPopulator
{
    public IReadOnlyList<IEntity> Populate(
        SectorId sector,
        long gameSeconds,
        ulong worldSeed,
        IEnumerable<Intervention> interventions)
    {
        var context = contextGen.Generate(sector, gameSeconds, worldSeed);
        var sectorRng = new Random(HashCode.Combine(sector.X, sector.Y, sector.Z, worldSeed));

        var results = new List<IEntity>();

        int asteroidCount = (int)Math.Round(context.AsteroidDensity * 20);
        var asteroidProto = registry.Get("enriched-asteroid"); // <- правильный ключ
    
        if (asteroidProto == null)
            return results; // или throw, если прототип обязателен

        for (int i = 0; i < asteroidCount; i++)
        {
            var instanceId = new EntityId($"asteroid-{sector.X}_{sector.Y}_{sector.Z}-{i}");
            var asteroidRng = new Random(HashCode.Combine(instanceId.Value, worldSeed)); // <- опять создание Random
            var asteroidInterventions = interventions
                .Where(iv => iv.EntityId.Equals(instanceId) && iv.GameSeconds <= gameSeconds)
                .ToList();
            
            var asteroid = asteroidProto.Instantiate(
                instanceId, sector, gameSeconds, context, asteroidInterventions, asteroidRng);
            results.Add(asteroid);
        }

        return results;
    }
}