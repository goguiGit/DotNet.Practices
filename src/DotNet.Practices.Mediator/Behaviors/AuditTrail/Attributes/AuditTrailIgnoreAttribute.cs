namespace DotNet.Practices.Mediator.Behaviors.AuditTrail.Attributes;

// Excludes the property from PayloadJson and ResponseJson, e.g. large bodies or sensitive fields.
[AttributeUsage(AttributeTargets.Property)]
public sealed class AuditTrailIgnoreAttribute : Attribute;
