namespace DotNet.Practices.Mediator.Behaviors.AuditTrail.Interfaces;

public interface IAuditSink
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
