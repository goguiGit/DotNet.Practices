using System.Net;
using DotNet.Practices.Abstractions.RequestPipeline.Interfaces;

namespace DotNet.Practices.Abstractions.RequestPipeline.Middlewares;

public class AuthenticationMiddleware : IMiddleware
{
    public Task InvokeAsync(RequestContext context, RequestDelegate next)
    {
        if (context.User is null)
        {
            context.StatusCode = HttpStatusCode.Unauthorized;
            context.Trace.Add("authentication: rejected anonymous request");
            return Task.CompletedTask;
        }

        context.Trace.Add($"authentication: {context.User} authenticated");
        return next(context);
    }
}
