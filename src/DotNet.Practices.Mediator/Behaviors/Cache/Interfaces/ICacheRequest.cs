namespace DotNet.Practices.Mediator.Behaviors.Cache.Interfaces;

public interface ICacheRequest
{
    string CacheKey { get; }

    TimeSpan? CacheExpiration { get; }
}
