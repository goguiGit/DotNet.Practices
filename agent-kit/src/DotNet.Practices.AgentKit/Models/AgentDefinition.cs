namespace DotNet.Practices.AgentKit.Models;

public sealed record AgentDefinition(string Name, string Description, string Model, bool ReadOnly, IReadOnlyList<string> Tools, string Body)
{
    // Records compare lists by reference; tools are part of the definition, so compare them by content.
    public bool Equals(AgentDefinition? other) =>
        other is not null
        && Name == other.Name
        && Description == other.Description
        && Model == other.Model
        && ReadOnly == other.ReadOnly
        && Tools.SequenceEqual(other.Tools, StringComparer.Ordinal)
        && Body == other.Body;

    public override int GetHashCode() => HashCode.Combine(Name, Description, Model, ReadOnly, string.Join(",", Tools), Body);
}
