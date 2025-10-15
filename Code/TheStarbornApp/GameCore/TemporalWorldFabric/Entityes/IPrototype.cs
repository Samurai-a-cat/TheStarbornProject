using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public interface IPrototype
{
    string PrototypeName { get; }
    IReadOnlyDictionary<string, PropValue> Props { get; }
    IEntity Instantiate(
        EntityId instanceId,
        SectorId sector,
        long gameSeconds,
        SharedContext context,
        IEnumerable<Intervention> interventions,
        Random rng);
}