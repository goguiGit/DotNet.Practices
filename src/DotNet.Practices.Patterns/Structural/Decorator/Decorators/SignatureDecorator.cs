using DotNet.Practices.Patterns.Structural.Decorator.Abstracts;
using DotNet.Practices.Patterns.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Decorator.Decorators;

public sealed class SignatureDecorator(IDocument inner, string signer) : DocumentDecorator(inner)
{
    public override string Render() => $"{base.Render()}\nSigned by {signer}";
}
