namespace DotNet.Practices.ScriptedMapping.Evaluation;

public sealed record EvaluationContext(
    IReadOnlyDictionary<string, object?>? Owner = null,
    IReadOnlyDictionary<string, object?>? Inputs = null,
    IReadOnlyDictionary<string, object?>? Options = null)
{
    public IReadOnlyDictionary<string, object?> Owner { get; } = Owner ?? new Dictionary<string, object?>();

    public IReadOnlyDictionary<string, object?> Inputs { get; } = Inputs ?? new Dictionary<string, object?>();

    public IReadOnlyDictionary<string, object?> Options { get; } = Options ?? new Dictionary<string, object?>();
}
