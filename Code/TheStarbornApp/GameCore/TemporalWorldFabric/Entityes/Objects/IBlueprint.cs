// File: TheStarbornApp/GameCore/TemporalWorldFabric/Entityes/Objects/StellarObjects/IBlueprint.cs

using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects;

/// <summary>
/// Обобщённый интерфейс для блюпринтов, определяющих способ создания сущностей.
/// </summary>
/// <typeparam name="T">Тип создаваемой сущности.</typeparam>
public interface IBlueprint<T> where T : class, IEntity
{
    
    /// <summary>
    /// Создаёт сущность на основе шаблона и контекста.
    /// Дефолтная реализация может содержать общую логику.
    /// </summary>
    /// <param name="id">ID создаваемой сущности.</param>
    /// <param name="context">Контекст сектора.</param>
    /// <param name="rng">Генератор случайных чисел.</param>
    /// <returns>Созданная сущность или null.</returns>
    T? CreateEntity(EntityId id, SharedContext context, Random rng)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (rng == null) throw new ArgumentNullException(nameof(rng));
        return CreateCustomEntity(id, context, rng);
    }

    /// <summary>
    /// Абстрактный метод, который должен быть реализован в каждом конкретном блюпринте.
    /// Содержит логику, специфичную для создания конкретного типа сущности.
    /// </summary>
    /// <param name="id">ID создаваемой сущности.</param>
    /// <param name="context">Контекст сектора.</param>
    /// <param name="rng">Генератор случайных чисел.</param>
    /// <returns>Созданная сущность или null.</returns>
    T? CreateCustomEntity(EntityId id, SharedContext context, Random rng);
}