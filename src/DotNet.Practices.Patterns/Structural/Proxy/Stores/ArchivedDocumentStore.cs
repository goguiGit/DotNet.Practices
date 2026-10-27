using DotNet.Practices.Patterns.Structural.Proxy.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Proxy.Stores;

// Real subject. Loads is exposed so a test can prove the proxy spared a round trip.
public sealed class ArchivedDocumentStore : IDocumentStore
{
    private readonly Dictionary<string, string> _documents = [];

    public int Loads { get; private set; }

    public void Store(string reference, string content) => _documents[reference] = content;

    public string Read(string reference)
    {
        Loads++;
        return _documents[reference];
    }
}
