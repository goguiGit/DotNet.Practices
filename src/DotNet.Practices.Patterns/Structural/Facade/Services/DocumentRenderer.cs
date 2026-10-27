namespace DotNet.Practices.Patterns.Structural.Facade.Services;

public sealed class DocumentRenderer
{
    public string Render(string title, string body) => $"{title}\n{body}";
}
