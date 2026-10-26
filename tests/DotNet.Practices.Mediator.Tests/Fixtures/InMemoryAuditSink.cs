using DotNet.Practices.Mediator.Behaviors.AuditTrail;
using DotNet.Practices.Mediator.Behaviors.AuditTrail.Interfaces;

namespace DotNet.Practices.Mediator.Tests.Fixtures;

public class InMemoryAuditSink : IAuditSink
{
    private readonly List<AuditEntry> _entries = [];

    public IReadOnlyList<AuditEntry> Entries => _entries;

    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        _entries.Add(entry);
        return Task.CompletedTask;
    }
}
