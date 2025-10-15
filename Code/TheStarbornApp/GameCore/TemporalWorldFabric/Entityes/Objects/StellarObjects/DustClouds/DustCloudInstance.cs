using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;

/// <summary>
/// Конкретный инстанс пылевого облака, созданный на основе шаблона и контекста.
/// Хранит уникальное состояние для этого экземпляра.
/// </summary>
public sealed class DustCloudInstance : IEntity, IDustCloudMarker
{
    public EntityId Id { get; }
    public string Name { get; }
    public int BaseDustAmount { get; }
    public int ValuableResourceAmount { get; }

    public DustCloudInstance(EntityId id, int baseDustAmount, int valuableResourceAmount)
    {
        Id = id;
        BaseDustAmount = baseDustAmount;
        ValuableResourceAmount = valuableResourceAmount;
        Name = $"Dust Cloud ({BaseDustAmount} dust, {ValuableResourceAmount} valuable)";
    }

    public IReadOnlyDictionary<string, PropValue> Props => new Dictionary<string, PropValue>
    {
        ["BaseDustAmount"] = new FixedValue(BaseDustAmount),
        ["ValuableResourceAmount"] = new FixedValue(ValuableResourceAmount),
        ["TotalMass"] = new FixedValue(BaseDustAmount + ValuableResourceAmount * 10) // Пример: ценные ресурсы тяжелее
    };
}