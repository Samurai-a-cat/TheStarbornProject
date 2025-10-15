using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.Asteroids;

public sealed class ProtoEnrichedAsteroid : IPrototype, IAsteroidMarker, IPrototypeMarker
{
    public string PrototypeName => "enriched-asteroid";
    
    // Статические пропы - общие для всех астероидов
    public static readonly IReadOnlyDictionary<string, PropValue> StaticProps = 
        new Dictionary<string, PropValue>
        {
            ["Mass"] = new FixedValue(1e12),
            ["ScanDifficulty"] = new FixedValue(0.2)
        };
    
    public IReadOnlyDictionary<string, PropValue> Props => StaticProps;

    public IEntity Instantiate(
        EntityId instanceId,
        SectorId sector,
        long gameSeconds,
        SharedContext context,
        IEnumerable<Intervention> interventions,
        Random rng)
    {
        // Читаем параметры из контекста или генерим рандомно
        var oreType = GetOreTypeFromContext(context, rng); // или из Props
        var baseAmount = 1000; // или из Props
        
        var hasResource = rng.NextDouble() < context.ResourceRichness;
        var oreAmount = hasResource ? (int)(baseAmount * context.ResourceAbundance) : 0;
        
        return new EnrichedAsteroid(instanceId, this, oreType, oreAmount);
    }
    
    private string GetOreTypeFromContext(SharedContext context, Random rng)
    {
        // Тут логика выбора типа руды по контексту
        // или просто рандом, или из каких-то параметров
        return "iron"; // для примера
    }
}

// EnrichedAsteroid остается, но теперь работает с IPrototype
public sealed class EnrichedAsteroid : IEntity, IAsteroidMarker
{
    private readonly IPrototype _prototype; // <- IPrototype, а не базовый класс
    
    public EntityId Id { get; }
    public string OreType { get; }
    public int OreAmount { get; }

    public EnrichedAsteroid(EntityId id, IPrototype prototype, string oreType, int oreAmount)
    {
        Id = id;
        _prototype = prototype ?? throw new ArgumentNullException(nameof(prototype));
        OreType = oreType ?? throw new ArgumentNullException(nameof(oreType));
        OreAmount = oreAmount;
    }

    public string Name => $"{_prototype.PrototypeName} ({OreAmount} {OreType})";
    public IReadOnlyDictionary<string, PropValue> Props => _prototype.Props;
}