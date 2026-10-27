namespace DotNet.Practices.Patterns.Structural.Facade.Services;

public sealed class DocumentArchive
{
    private readonly Dictionary<string, string> _documents = [];

    public void Store(string reference, string content) => _documents[reference] = content;

    public string Read(string reference) => _documents[reference];
}
