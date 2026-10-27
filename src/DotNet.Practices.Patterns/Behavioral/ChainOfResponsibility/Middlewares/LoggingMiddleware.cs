using DotNet.Practices.Patterns.Behavioral.ChainOfResponsibility.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.ChainOfResponsibility.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(RequestContext context, RequestDelegate next)
    {
        context.Trace.Add($"logging: started {context.Path}");
        await next(context);
        context.Trace.Add($"logging: completed {context.Path} with {context.StatusCode}");
    }
}
