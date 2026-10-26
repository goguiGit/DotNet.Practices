namespace DotNet.Practices.Mediator.Behaviors.AuditTrail;

public record AuditEntry(
    string CommandType,
    string FriendlyName,
    AuditActionType Action,
    string PayloadJson,
    string ResponseJson,
    bool Succeeded,
    DateTimeOffset Timestamp);
