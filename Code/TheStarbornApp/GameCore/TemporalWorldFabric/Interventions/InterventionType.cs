using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;

public record Intervention(
    EntityId EntityId,
    SectorId Sector,
    long GameSeconds,
    InterventionType Type,
    Dictionary<string, object> Payload //Todo: в будущем переработать эту хуйню, чтоб работать не с строчками
);

public enum InterventionType
{
    MinedAsteroid
}