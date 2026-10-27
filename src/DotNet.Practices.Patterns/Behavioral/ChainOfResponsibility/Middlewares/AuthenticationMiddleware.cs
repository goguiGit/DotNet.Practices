using System.Net;
using DotNet.Practices.Patterns.Behavioral.ChainOfResponsibility.Interfaces;

namespace DotNet.Practices.Patterns.Behavioral.ChainOfResponsibility.Middlewares;

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
