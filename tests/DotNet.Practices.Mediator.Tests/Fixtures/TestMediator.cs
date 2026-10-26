using DotNet.Practices.Mediator.Behaviors.AuditTrail.Interfaces;
using DotNet.Practices.Mediator.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Practices.Mediator.Tests.Fixtures;

public sealed class TestMediator
{
    public GetGreetingQueryHandler GreetingHandler { get; } = new();

    public RenamePetCommandHandler RenameHandler { get; } = new();

    public InMemoryAuditSink AuditSink { get; } = new();

    public IMediator Mediator { get; }

    public TestMediator()
    {
        var services = new ServiceCollection();
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<TestMediator>());
        services.AddMediatorBehaviors();
        services.AddSingleton<IAuditSink>(AuditSink);
        services.AddSingleton<IRequestHandler<GetGreetingQuery, string>>(GreetingHandler);
        services.AddSingleton<IRequestHandler<RenamePetCommand, string>>(RenameHandler);

        Mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
