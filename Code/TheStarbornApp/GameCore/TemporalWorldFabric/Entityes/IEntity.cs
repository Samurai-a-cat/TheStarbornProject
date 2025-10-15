namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

/// <summary>
/// Базовый контракт игровой сущности.
/// Сущность — это неизменяемый результат генерации (инстанс),
/// созданный из прототипа с учётом контекста, времени и вмешательств.
/// </summary>
public interface IEntity
{
    EntityId Id { get; }
    string Name { get; }
    IReadOnlyDictionary<string, PropValue> Props { get; }
}

/// <summary>
/// Типизированное значение свойства сущности.
/// Используется для декларативного описания характеристик прототипов
/// (например, радиус планеты, сложность сканирования астероида).
/// Поддерживает фиксированные значения, флаги и диапазоны.
/// </summary>
public abstract record PropValue;

public sealed record FixedValue(double Value) : PropValue;
public sealed record FlagValue(bool Value) : PropValue;

public sealed record RangeValue(double Min, double Max) : PropValue
{
    public double Average => (Min + Max) / 2.0;
}

/// <summary>
/// Типобезопасный идентификатор сущности.
/// Гарантирует отсутствие опечаток при работе с типами объектов
/// и обеспечивает совместимость с системой регистрации прототипов.
/// </summary>
public readonly record struct EntityId(string Value) : IEquatable<EntityId>
{
    public static implicit operator EntityId(string value) => new(value);
    public static explicit operator string(EntityId id) => id.Value;
}