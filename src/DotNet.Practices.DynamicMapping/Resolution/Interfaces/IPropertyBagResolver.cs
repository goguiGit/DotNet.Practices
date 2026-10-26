using DotNet.Practices.DynamicMapping.Models;

namespace DotNet.Practices.DynamicMapping.Resolution.Interfaces;

public interface IPropertyBagResolver
{
    Task<ResolvedBag> ResolveAsync(PropertyBag bag, MappingContext context, CancellationToken cancellationToken = default);
}
