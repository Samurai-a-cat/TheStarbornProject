namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

public record SharedContext(
    float AsteroidDensity,        // 0.0–1.0 → вероятность спавна астероида
    float ResourceRichness,       // 0.0–1.0 → шанс, что астероид содержит ресурс
    float ResourceAbundance       // 0.0–1.0 → множитель количества ресурса
);