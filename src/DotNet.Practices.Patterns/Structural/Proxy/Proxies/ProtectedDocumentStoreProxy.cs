using DotNet.Practices.Patterns.Structural.Proxy.Interfaces;

namespace DotNet.Practices.Patterns.Structural.Proxy.Proxies;

public sealed class ProtectedDocumentStoreProxy(IDocumentStore inner, string role) : IDocumentStore
{
    public string Read(string reference) => role == "HR"
        ? inner.Read(reference)
        : throw new UnauthorizedAccessException($"Role '{role}' cannot read payroll documents.");
}
