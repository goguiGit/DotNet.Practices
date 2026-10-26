namespace DotNet.Practices.ScriptedMapping.Scripting;

public static class ExpressionTranslator
{
    public static bool IsScript(string? expression) => expression is not null && expression.StartsWith('$');

    // Rabbit drops the leading $ and turns every apostrophe into a double quote, so `$ $'CN={X}'` becomes
    // the interpolated string ` $"CN={X}"`. Side effect inherited on purpose: no char literals, no apostrophes.
    public static string ToScript(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        if (!IsScript(expression))
        {
            throw new ArgumentException("Only expressions starting with $ are scripts.", nameof(expression));
        }

        return expression[1..].Replace('\'', '"');
    }
}
