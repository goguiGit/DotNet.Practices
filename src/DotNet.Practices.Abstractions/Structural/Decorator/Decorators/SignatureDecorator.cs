using DotNet.Practices.Abstractions.Structural.Decorator.Abstracts;
using DotNet.Practices.Abstractions.Structural.Decorator.Interfaces;

namespace DotNet.Practices.Abstractions.Structural.Decorator.Decorators;

public sealed class SignatureDecorator(IDocument inner, string signer) : DocumentDecorator(inner)
{
    public override string Render() => $"{base.Render()}\nSigned by {signer}";
}
