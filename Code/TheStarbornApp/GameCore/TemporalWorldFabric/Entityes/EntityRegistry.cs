using System.Reflection;
using TheStarbornApp.GameCore.Entityes;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public sealed class EntityRegistry
{
    private readonly Dictionary<EntityId, Type> _entityTypes;

    public EntityRegistry(Assembly assembly)
    {
        _entityTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IEntity).IsAssignableFrom(t)
                        && typeof(ITemporalEntity).IsAssignableFrom(t))
            .ToDictionary(
                t => ((IEntity)Activator.CreateInstance(t)!).Id,
                t => t
            );
    }

    public T? Create<T>(EntityId id) where T : class, IEntity
        => _entityTypes.TryGetValue(id, out var type)
            ? Activator.CreateInstance(type) as T
            : null;
}