using DotNet.Practices.Mediator.Behaviors.AuditTrail;
using DotNet.Practices.Mediator.Behaviors.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Practices.Mediator.Extensions;

public static class ServiceCollectionExtensions
{
    // Registration order is execution order: audit wraps caching so a cache hit is still audited.
    public static IServiceCollection AddMediatorBehaviors(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditCommandBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        return services;
    }
}
