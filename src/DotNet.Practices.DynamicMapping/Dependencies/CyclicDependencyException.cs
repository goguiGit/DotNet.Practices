namespace DotNet.Practices.DynamicMapping.Dependencies;

public sealed class CyclicDependencyException(IReadOnlyCollection<string> propertyIds)
    : InvalidOperationException($"Properties depend on each other in a cycle: {string.Join(", ", propertyIds)}.")
{
    public IReadOnlyCollection<string> PropertyIds { get; } = propertyIds;
}
