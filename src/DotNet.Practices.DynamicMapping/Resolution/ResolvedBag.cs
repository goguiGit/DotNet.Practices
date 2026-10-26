namespace DotNet.Practices.DynamicMapping.Resolution;

public record ResolvedBag(string BagId, IReadOnlyDictionary<string, string?> Values, string RawResponse);
