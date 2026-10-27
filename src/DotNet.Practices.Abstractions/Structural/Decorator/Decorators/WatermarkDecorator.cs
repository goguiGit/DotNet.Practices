using DotNet.Practices.Abstractions.Structural.Decorator.Abstracts;
using DotNet.Practices.Abstractions.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Decorator.Decorators;

public sealed class WatermarkDecorator(IDocument inner, string watermark) : DocumentDecorator(inner)
{
    public override string Render() => $"{base.Render()}\n[{watermark}]";
}
