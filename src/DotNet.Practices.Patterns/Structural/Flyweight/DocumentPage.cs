namespace DotNet.Practices.Patterns.Structural.Flyweight;

public sealed class DocumentPage(TextStyleFactory styles)
{
    private readonly List<StyledRun> _runs = [];

    public IReadOnlyList<StyledRun> Runs => _runs;

    public void Write(string text, string font, int size, bool isBold) =>
        _runs.Add(new StyledRun(text, styles.Get(font, size, isBold)));
}
