using DotNet.Practices.Mediator.Abstractions.Messaging;
using DotNet.Practices.Mediator.Behaviors.Cache.Interfaces;

namespace DotNet.Practices.Mediator.Tests.Fixtures;

public record GetGreetingQuery(string Name) : IQuery<string>, ICacheRequest
{
    public string CacheKey => $"greeting:{Name}";

    public TimeSpan? CacheExpiration => null;
}

public class GetGreetingQueryHandler : IQueryHandler<GetGreetingQuery, string>
{
    public int Calls { get; private set; }

    public Task<string> Handle(GetGreetingQuery request, CancellationToken cancellationToken)
    {
        Calls++;
        return Task.FromResult($"Hello {request.Name}");
    }
}
