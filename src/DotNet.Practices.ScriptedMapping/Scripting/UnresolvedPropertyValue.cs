namespace DotNet.Practices.ScriptedMapping.Scripting;

// What a script reads when it touches a property whose generator has not run yet.
public sealed class UnresolvedPropertyValue
{
    public static readonly UnresolvedPropertyValue Instance = new();

    private UnresolvedPropertyValue()
    {
    }

    public override string ToString() => "<unresolved>";
}
