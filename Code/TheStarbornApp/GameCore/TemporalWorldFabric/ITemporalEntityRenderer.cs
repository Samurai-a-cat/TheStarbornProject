using TheStarbornApp.GameCore.Entityes;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric;

public interface ITemporalEntityRenderer
{
    IEntity Render(
        EntityId id,
        SectorId sector,
        long gameSeconds,
        ulong worldSeed,
        IEnumerable<Intervention> interventions
    );
}