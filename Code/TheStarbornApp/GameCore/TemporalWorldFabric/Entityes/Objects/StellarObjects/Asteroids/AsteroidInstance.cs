// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/Asteroids/AsteroidInstance.cs
namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;

/// <summary>
/// Конкретный инстанс астероида, созданный на основе шаблона и контекста.
/// Хранит уникальное состояние для этого экземпляра.
/// </summary>
public sealed class AsteroidInstance : IEntity, IAsteroidMarker
{
    public EntityId Id { get; }
    public string Name { get; }
    public string OreType { get; }
    public int OreAmount { get; }
    public double Mass { get; }
    public double Density { get; }

    // Конструктор принимает вычисленные значения, а не шаблон
    public AsteroidInstance(EntityId id, string oreType, int oreAmount, double baseMass, double density)
    {
        Id = id;
        OreType = oreType ?? throw new ArgumentNullException(nameof(oreType));
        OreAmount = oreAmount;
        Mass = baseMass + (OreAmount * 0.1); // Пример: масса зависит от количества руды
        Density = density;
        Name = $"Asteroid ({OreAmount} {OreType})";
    }

    // Вычисляем Props на лету, основываясь на состоянии инстанса
    public IReadOnlyDictionary<string, PropValue> Props => new Dictionary<string, PropValue>
    {
        ["Mass"] = new FixedValue(Mass),
        ["Density"] = new FixedValue(Density),
        ["OreAmount"] = new FixedValue(OreAmount),
        ["OreType"] = new FixedValue(OreType.GetHashCode()) // Или как-то иначе, если строку нельзя
    };
}