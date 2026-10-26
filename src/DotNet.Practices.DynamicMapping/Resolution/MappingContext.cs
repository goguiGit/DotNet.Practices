namespace DotNet.Practices.DynamicMapping.Resolution;

// Owner is the external record (e.g. the HR row); Inputs are the values the requester typed in the form;
// Options holds provisioning containers and lookup values.
public record MappingContext(
    IReadOnlyDictionary<string, string> Owner,
    IReadOnlyDictionary<string, string> Inputs,
    IReadOnlyDictionary<string, string> Options);
