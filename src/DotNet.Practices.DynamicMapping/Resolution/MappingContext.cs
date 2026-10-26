namespace DotNet.Practices.DynamicMapping.Resolution;

public sealed record MappingContext(
    IReadOnlyDictionary<string, string> Owner,
    IReadOnlyDictionary<string, string> Inputs,
    IReadOnlyDictionary<string, string> Options);
