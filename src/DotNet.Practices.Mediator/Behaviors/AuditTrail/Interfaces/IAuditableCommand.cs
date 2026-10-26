using DotNet.Practices.Mediator.Abstractions.Messaging;

namespace DotNet.Practices.Mediator.Behaviors.AuditTrail.Interfaces;

public interface IAuditableCommand : ICommand
{
    string FriendlyName { get; }

    AuditActionType Action { get; }
}
