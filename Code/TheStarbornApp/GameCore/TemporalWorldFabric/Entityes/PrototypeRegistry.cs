using System.Reflection;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.TemporalWorldFabric.Interventions;

namespace TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

public sealed class PrototypeRegistry
{
    private readonly Dictionary<string, IPrototype> _prototypes;

    public PrototypeRegistry(Assembly assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        var prototypeTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IPrototype).IsAssignableFrom(t)
                        && typeof(IPrototypeMarker).IsAssignableFrom(t))
            .ToList();

        Console.WriteLine($"Found {prototypeTypes.Count} types that match IPrototype and IPrototypeMarker.");

        _prototypes = new Dictionary<string, IPrototype>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in prototypeTypes)
        {
            try
            {
                var prototype = (IPrototype)Activator.CreateInstance(type)!;
                Console.WriteLine($"Instantiated: {type.Name} -> {prototype.PrototypeName}");
                if (_prototypes.ContainsKey(prototype.PrototypeName))
                {
                    throw new InvalidOperationException(
                        $"Duplicate prototype name '{prototype.PrototypeName}' found in type {type.Name}");
                }
                _prototypes[prototype.PrototypeName] = prototype;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to instantiate prototype from type {type.Name}: {ex.Message}", ex);
            }
        }

        Console.WriteLine($"Registered {_prototypes.Count} prototypes.");
    }

    public IPrototype? Get(string prototypeName) => 
        _prototypes.GetValueOrDefault(prototypeName);

    public IReadOnlyCollection<string> GetAllPrototypeNames() => 
        _prototypes.Keys.ToList().AsReadOnly();
}