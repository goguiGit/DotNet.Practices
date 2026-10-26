using DotNet.Practices.Abstractions.RequestPipeline.Interfaces;

namespace DotNet.Practices.Abstractions.RequestPipeline;

public class PipelineBuilder
{
    private readonly List<IMiddleware> _middlewares = [];

    public PipelineBuilder Use(IMiddleware middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        _middlewares.Add(middleware);
        return this;
    }

    public RequestDelegate Build(RequestDelegate terminal)
    {
        ArgumentNullException.ThrowIfNull(terminal);

        // Compose from the end so the first registered middleware is the outermost call.
        var next = terminal;
        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            var middleware = _middlewares[i];
            var downstream = next;
            next = context => middleware.InvokeAsync(context, downstream);
        }

        return next;
    }
}
