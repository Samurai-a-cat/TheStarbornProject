// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/GasClouds/GasCloudEntity.cs
namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.GasClouds;
/// <summary>
/// Конкретный инстанс газового облака, созданный на основе шаблона и контекста.
/// Хранит уникальное состояние для этого экземпляра.
/// </summary>
public sealed class GasCloudInstance : IEntity, IGasCloudMarker // Переименовано
{
    public EntityId Id { get; }
    public string Name { get; }
    public string GasType { get; }
    public int GasAmount { get; }
    public double Density { get; }
    // Конструктор принимает вычисленные значения, а не шаблон
    public GasCloudInstance(EntityId id, string gasType, int gasAmount, double baseDensity)
    {
        Id = id;
        GasType = gasType ?? throw new ArgumentNullException(nameof(gasType));
        GasAmount = gasAmount;
        Density = baseDensity + (GasAmount * 0.01); // Пример: плотность зависит от количества газа
        Name = $"Gas Cloud ({GasAmount} units of {GasType})";
    }
    // Вычисляем Props на лету, основываясь на состоянии инстанса
    public IReadOnlyDictionary<string, PropValue> Props => new Dictionary<string, PropValue>
    {
        ["Density"] = new FixedValue(Density),
        ["GasAmount"] = new FixedValue(GasAmount),
        ["GasType"] = new FixedValue(GasType.GetHashCode()) // Или как-то иначе, если строку нельзя
    };
}