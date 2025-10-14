using TheStarbornApp.GameCore.Entityes;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public interface ITemporalEntity : IEntity
{
    IEntity Enrich(
        EntityId id,
        SectorId sector,
        long gameSeconds,
        SharedContext context,
        IEnumerable<Intervention> interventions,
        Random deterministicRng
    );
}