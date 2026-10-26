namespace DotNet.Practices.Abstractions.RequestPipeline.Interfaces;

public interface IMiddleware
{
    Task InvokeAsync(RequestContext context, RequestDelegate next);
}
