using DotNet.Practices.Patterns.Structural.Decorator.Abstracts;
using DotNet.Practices.Patterns.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Decorator.Decorators;

public sealed class WatermarkDecorator(IDocument inner, string watermark) : DocumentDecorator(inner)
{
    public override string Render() => $"{base.Render()}\n[{watermark}]";
}
