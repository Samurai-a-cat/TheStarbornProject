namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;

public sealed class DustCloudTemplateData : ITemplateData
{
    public string TemplateName { get; set; } = string.Empty;
    public double BaseDustAmount { get; set; }
    public double ValuableResourceMultiplier { get; set; }
    public double SpawnChance { get; set; }
    public double OptimalDistance { get; set; }
    public double DistanceInfluenceFactor { get; set; }
}