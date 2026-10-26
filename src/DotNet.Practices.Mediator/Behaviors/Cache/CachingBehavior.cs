using DotNet.Practices.Mediator.Behaviors.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DotNet.Practices.Mediator.Behaviors.Cache;

public class CachingBehavior<TRequest, TResponse>(IMemoryCache cache) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromDays(1);

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheRequest cacheRequest)
        {
            return await next();
        }

        if (cache.TryGetValue(cacheRequest.CacheKey, out TResponse? cached) && cached is not null)
        {
            return cached;
        }

        var response = await next();
        cache.Set(cacheRequest.CacheKey, response, cacheRequest.CacheExpiration ?? DefaultExpiration);
        return response;
    }
}
