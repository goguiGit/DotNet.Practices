namespace DotNet.Practices.Patterns.Behavioral.ChainOfResponsibility.Interfaces;

public interface IMiddleware
{
    Task InvokeAsync(RequestContext context, RequestDelegate next);
}
