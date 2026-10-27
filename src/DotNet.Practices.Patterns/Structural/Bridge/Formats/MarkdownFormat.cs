using DotNet.Practices.Patterns.Structural.Bridge.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Bridge.Formats;

public sealed class MarkdownFormat : IDocumentFormat
{
    public string Title(string text) => $"# {text}";

    public string Line(string label, string value) => $"**{label}**: {value}";
}
