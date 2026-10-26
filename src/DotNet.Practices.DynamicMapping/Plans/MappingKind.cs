namespace DotNet.Practices.DynamicMapping.Plans;

public enum MappingKind
{
    // Deterministic expression in the mini-DSL; runs without the model.
    Expression,

    // Resolved against a lookup table; runs without the model.
    Lookup,

    // Needs the model on every record (free text, semantic choice).
    Dynamic,

    // The compiler could not produce a mapping; left for a human.
    Unmapped,
}
