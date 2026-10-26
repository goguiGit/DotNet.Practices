namespace DotNet.Practices.ScriptedMapping.Evaluation;

public sealed class ScriptCompilationException(string bagId, string propertyId, string expression, Exception innerException)
    : Exception($"Property '{propertyId}' of bag '{bagId}' does not compile: {expression}", innerException)
{
    public string BagId { get; } = bagId;

    public string PropertyId { get; } = propertyId;
}
