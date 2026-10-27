namespace DotNet.Practices.Patterns.Structural.Bridge.Interfaces;

// Implementation side of the bridge: the primitives every format must know how to draw.
public interface IDocumentFormat
{
    string Title(string text);

    string Line(string label, string value);
}
