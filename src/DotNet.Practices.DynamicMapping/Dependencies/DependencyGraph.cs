using DotNet.Practices.DynamicMapping.Models;

namespace DotNet.Practices.DynamicMapping.Dependencies;

public sealed class DependencyGraph
{
    private readonly IReadOnlyList<string> _declarationOrder;
    private readonly IReadOnlyDictionary<string, IReadOnlySet<string>> _dependencies;
    private readonly IReadOnlyDictionary<string, IReadOnlySet<string>> _externalReferences;

    private DependencyGraph(
        IReadOnlyList<string> declarationOrder,
        IReadOnlyDictionary<string, IReadOnlySet<string>> dependencies,
        IReadOnlyDictionary<string, IReadOnlySet<string>> externalReferences)
    {
        _declarationOrder = declarationOrder;
        _dependencies = dependencies;
        _externalReferences = externalReferences;
    }

    public static DependencyGraph Build(PropertyBag bag)
    {
        ArgumentNullException.ThrowIfNull(bag);

        var ids = bag.Properties.Select(property => property.Id).ToList();
        var known = ids.ToHashSet(StringComparer.Ordinal);
        var dependencies = new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal);
        var externalReferences = new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal);

        foreach (var property in bag.Properties)
        {
            var references = new[] { property.DefaultValue, property.ValueGenerator, property.IdentifyingGenerator }
                .SelectMany(DependencyExtractor.Extract)
                .ToHashSet(StringComparer.Ordinal);

            dependencies[property.Id] = references.Where(known.Contains).ToHashSet(StringComparer.Ordinal);
            externalReferences[property.Id] = references.Where(reference => !known.Contains(reference)).ToHashSet(StringComparer.Ordinal);
        }

        return new DependencyGraph(ids, dependencies, externalReferences);
    }

    public IReadOnlySet<string> DependenciesOf(string propertyId) => _dependencies[propertyId];

    public IReadOnlySet<string> ExternalReferencesOf(string propertyId) => _externalReferences[propertyId];

    // Topological order that keeps declaration order whenever dependencies allow it.
    public IReadOnlyList<string> EvaluationOrder()
    {
        var ordered = new List<string>(_declarationOrder.Count);
        var emitted = new HashSet<string>(StringComparer.Ordinal);
        var remaining = _declarationOrder.ToList();

        while (remaining.Count > 0)
        {
            var next = remaining.FirstOrDefault(id => _dependencies[id].All(emitted.Contains))
                ?? throw new CyclicDependencyException(remaining);

            ordered.Add(next);
            emitted.Add(next);
            remaining.Remove(next);
        }

        return ordered;
    }
}
