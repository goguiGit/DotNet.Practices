using Microsoft.Extensions.AI;

namespace DotNet.Practices.DynamicMapping.Resolution;

public sealed record ResolvedBag(string BagId, IReadOnlyDictionary<string, string?> Values, string RawResponse, UsageDetails? Usage);
