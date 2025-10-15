namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

public sealed class DefaultSectorContextGenerator : ISectorContextGenerator
{
    public SharedContext Generate(SectorId sector, long gameSeconds, ulong seed)
    {
        // Детерминированный хеш из сектора и сида
        var hash = HashCode.Combine(sector.X, sector.Y, sector.Z, seed);
        var rng = new Random((int)hash); 

        // Простая логика: чем дальше от центра — тем беднее
        var distance = Math.Sqrt(sector.X * sector.X + sector.Y * sector.Y + sector.Z * sector.Z);
        var richness = Math.Max(0.1f, 1.0f - (float)(distance / 100.0));

        return new SharedContext(
            AsteroidDensity: Math.Clamp(richness + (float)rng.NextDouble() * 0.2f - 0.1f, 0.0f, 1.0f),
            ResourceRichness: richness,
            ResourceAbundance: richness,
            DustCloudSpawnModifier: richness
        );
    }
}