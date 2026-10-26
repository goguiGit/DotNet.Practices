using DotNet.Practices.Abstractions.RequestPipeline.Interfaces;

namespace DotNet.Practices.Abstractions.RequestPipeline.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(RequestContext context, RequestDelegate next)
    {
        context.Trace.Add($"logging: started {context.Path}");
        await next(context);
        context.Trace.Add($"logging: completed {context.Path} with {context.StatusCode}");
    }
}
