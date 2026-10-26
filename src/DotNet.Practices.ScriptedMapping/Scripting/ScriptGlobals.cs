namespace DotNet.Practices.ScriptedMapping.Scripting;

// The globals type every script is compiled against; `dynamic` is why typos only surface at runtime.
public sealed class ScriptGlobals(ScriptContext context, object services)
{
    public dynamic Context { get; } = context;

    public dynamic Services { get; } = services;
}
