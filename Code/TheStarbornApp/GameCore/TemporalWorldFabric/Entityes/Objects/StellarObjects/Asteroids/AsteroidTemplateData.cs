// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/Asteroids/AsteroidTemplateData.cs

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

public sealed class AsteroidTemplateData : ITemplateData
{
    public string TemplateName { get; set; } = string.Empty;
    public double BaseMass { get; set; }
    public double BaseScanDifficulty { get; set; }
    public string[] PossibleOreTypes { get; set; } = Array.Empty<string>();
    public double BaseOreAmountMin { get; set; }
    public double BaseOreAmountMax { get; set; }
    public double BaseDensity { get; set; }
}