namespace DotNet.Practices.ScriptedMapping.Evaluation;

public sealed class CircularDependencyException(IReadOnlyCollection<string> propertyIds)
    : InvalidOperationException($"No property can be evaluated next; remaining: {string.Join(", ", propertyIds)}.")
{
    public IReadOnlyCollection<string> PropertyIds { get; } = propertyIds;
}
