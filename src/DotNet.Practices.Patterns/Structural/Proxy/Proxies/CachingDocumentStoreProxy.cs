using DotNet.Practices.Patterns.Structural.Proxy.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Proxy.Proxies;

public sealed class CachingDocumentStoreProxy(IDocumentStore inner) : IDocumentStore
{
    private readonly Dictionary<string, string> _cache = [];

    public string Read(string reference)
    {
        if (!_cache.TryGetValue(reference, out var content))
        {
            content = inner.Read(reference);
            _cache[reference] = content;
        }

        return content;
    }
}
