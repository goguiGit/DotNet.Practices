namespace DotNet.Practices.Abstractions.Structural.Flyweight;

public sealed class TextStyleFactory
{
    private readonly Dictionary<TextStyle, TextStyle> _styles = [];

    public int Count => _styles.Count;

    public TextStyle Get(string font, int size, bool isBold)
    {
        var wanted = new TextStyle(font, size, isBold);
        if (_styles.TryGetValue(wanted, out var shared))
        {
            return shared;
        }

        _styles[wanted] = wanted;
        return wanted;
    }
}
