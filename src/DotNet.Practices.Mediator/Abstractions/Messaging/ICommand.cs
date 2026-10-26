namespace DotNet.Practices.Mediator.Abstractions.Messaging;

public interface ICommand;

public interface ICommand<out TResponse> : IRequest<TResponse>, ICommand;
