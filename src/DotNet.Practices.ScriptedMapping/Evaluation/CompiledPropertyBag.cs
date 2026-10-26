using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.ScriptedMapping.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DotNet.Practices.ScriptedMapping.Evaluation;

public sealed record CompiledProperty(string Id, ScriptRunner<object>? Script, object? Literal, IReadOnlySet<string> Dependencies)
{
    public bool HasGenerator => Script is not null || Literal is not null;
}

public sealed record CompiledPropertyBag(PropertyBag Bag, IReadOnlyList<CompiledProperty> Properties);
