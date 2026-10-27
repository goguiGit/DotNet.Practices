using System.Text;

namespace DotNet.Practices.Patterns.Structural.Adapter.Adaptees;

// Stands in for a third-party library we cannot change: stateful open/write/close instead of one call.
public sealed class LegacyPdfEngine
{
    private readonly StringBuilder _buffer = new();

    public void Open(string title) => _buffer.Clear().Append("%PDF ").Append(title);

    public void WriteLine(string text) => _buffer.Append('\n').Append(text);

    public string Close() => _buffer.Append("\n%%EOF").ToString();
}
