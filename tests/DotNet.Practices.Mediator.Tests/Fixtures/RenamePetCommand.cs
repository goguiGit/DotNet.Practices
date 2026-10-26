using DotNet.Practices.Mediator.Abstractions.Messaging;
using DotNet.Practices.Mediator.Behaviors.AuditTrail;
using DotNet.Practices.Mediator.Behaviors.AuditTrail.Attributes;
using DotNet.Practices.Mediator.Behaviors.AuditTrail.Interfaces;

namespace DotNet.Practices.Mediator.Tests.Fixtures;

public record RenamePetCommand(int PetId, string NewName, [property: AuditTrailIgnore] string Notes)
    : ICommand<string>, IAuditableCommand
{
    public string FriendlyName => "Rename pet";

    public AuditActionType Action => AuditActionType.Update;
}

public class RenamePetCommandHandler : ICommandHandler<RenamePetCommand, string>
{
    public int Calls { get; private set; }

    public Task<string> Handle(RenamePetCommand request, CancellationToken cancellationToken)
    {
        Calls++;
        if (string.IsNullOrWhiteSpace(request.NewName))
        {
            throw new InvalidOperationException("A pet needs a name.");
        }

        return Task.FromResult($"Pet {request.PetId} renamed to {request.NewName}");
    }
}
