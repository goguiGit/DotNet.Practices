namespace DotNet.Practices.Mediator.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>;
