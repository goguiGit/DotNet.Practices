using System.Collections.Concurrent;
using System.Globalization;
using DotNet.Practices.DynamicMapping.Dependencies;
using DotNet.Practices.DynamicMapping.Models;
using DotNet.Practices.ScriptedMapping.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp.RuntimeBinder;

namespace DotNet.Practices.ScriptedMapping.Evaluation;

public sealed class ScriptedPropertyBagEvaluator(object services)
{
    private static readonly ScriptOptions Options = ScriptOptions.Default
        .WithImports("System")
        .WithReferences(typeof(ScriptGlobals).Assembly, typeof(Binder).Assembly, typeof(System.Dynamic.DynamicObject).Assembly);

    private readonly ConcurrentDictionary<string, CompiledPropertyBag> _compiled = new(StringComparer.Ordinal);

    public CompiledPropertyBag Compile(PropertyBag bag)
    {
        ArgumentNullException.ThrowIfNull(bag);
        return _compiled.GetOrAdd(bag.Id, _ => new CompiledPropertyBag(bag, bag.Properties.Select(property => CompileProperty(bag, property)).ToList()));
    }

    public IReadOnlyDictionary<string, object?> Evaluate(PropertyBag bag, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var compiled = Compile(bag);

        var values = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var property in compiled.Properties)
        {
            values[property.Id] = property.HasGenerator ? UnresolvedPropertyValue.Instance : context.Inputs.GetValueOrDefault(property.Id);
        }

        // Rabbit's loop: take the first property whose dependencies are no longer pending, evaluate it, start over.
        var pending = compiled.Properties.Where(property => property.HasGenerator).ToList();
        while (pending.Count > 0)
        {
            var next = pending.FirstOrDefault(property => property.Dependencies.All(dependency => pending.All(other => other.Id != dependency)))
                ?? throw new CircularDependencyException(pending.Select(property => property.Id).ToList());

            values[next.Id] = Run(next, values, context);
            pending.Remove(next);
        }

        return values;
    }

    private object? Run(CompiledProperty property, Dictionary<string, object?> values, EvaluationContext context)
    {
        if (property.Script is null)
        {
            return property.Literal;
        }

        var members = new Dictionary<string, object?>(values, StringComparer.Ordinal)
        {
            ["Owner"] = context.Owner,
            ["Options"] = context.Options,
        };

        try
        {
            return property.Script(new ScriptGlobals(new ScriptContext(members), services)).GetAwaiter().GetResult();
        }
        catch (RuntimeBinderException)
        {
            // Rabbit logs a warning and leaves the property unset when a script dereferences null or a missing member.
            return null;
        }
    }

    private static CompiledProperty CompileProperty(PropertyBag bag, PropertyDefinition property)
    {
        var expression = property.IdentifyingGenerator ?? property.ValueGenerator ?? property.DefaultValue;
        if (expression is null)
        {
            return new CompiledProperty(property.Id, null, null, new HashSet<string>(StringComparer.Ordinal));
        }

        if (!ExpressionTranslator.IsScript(expression))
        {
            return new CompiledProperty(property.Id, null, ParseLiteral(expression), new HashSet<string>(StringComparer.Ordinal));
        }

        try
        {
            var script = CSharpScript.Create<object>(ExpressionTranslator.ToScript(expression), Options, typeof(ScriptGlobals)).CreateDelegate();
            return new CompiledProperty(property.Id, script, null, DependencyExtractor.Extract(expression));
        }
        catch (CompilationErrorException exception)
        {
            throw new ScriptCompilationException(bag.Id, property.Id, expression, exception);
        }
    }

    // Rabbit's ParseValue: bool, then int, then double, then the text itself.
    private static object ParseLiteral(string literal)
    {
        if (bool.TryParse(literal, out var boolean))
        {
            return boolean;
        }

        if (int.TryParse(literal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
        {
            return integer;
        }

        if (double.TryParse(literal, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
        {
            return number;
        }

        return literal;
    }
}
