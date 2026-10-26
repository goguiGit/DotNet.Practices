using System.Dynamic;

namespace DotNet.Practices.ScriptedMapping.Scripting;

public sealed class ScriptContext(IReadOnlyDictionary<string, object?> members) : DynamicObject
{
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = members.TryGetValue(binder.Name, out var value) ? value : null;
        if (result is IReadOnlyDictionary<string, object?> nested)
        {
            result = new ScriptContext(nested);
        }

        return true;
    }

    public override IEnumerable<string> GetDynamicMemberNames() => members.Keys;
}
