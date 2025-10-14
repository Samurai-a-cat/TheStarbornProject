namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

public readonly record struct SectorId(int X, int Y, int Z) : IEquatable<SectorId>;

public interface ISectorContextGenerator
{
    SharedContext Generate(SectorId sector, long gameSeconds, ulong worldSeed);
}