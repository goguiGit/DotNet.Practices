using DotNet.Practices.Patterns.Structural.Bridge.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Bridge.Formats;

public sealed class HtmlFormat : IDocumentFormat
{
    public string Title(string text) => $"<h1>{text}</h1>";

    public string Line(string label, string value) => $"<p>{label}: {value}</p>";
}
