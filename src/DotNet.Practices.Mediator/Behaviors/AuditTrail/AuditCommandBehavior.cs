using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using DotNet.Practices.Mediator.Behaviors.AuditTrail.Attributes;
using DotNet.Practices.Mediator.Behaviors.AuditTrail.Interfaces;

namespace DotNet.Practices.Mediator.Behaviors.AuditTrail;

public class AuditCommandBehavior<TRequest, TResponse>(IAuditSink auditSink) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { SkipIgnoredProperties } },
    };

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IAuditableCommand auditableCommand)
        {
            return await next();
        }

        TResponse response = default!;
        Exception? failure = null;
        try
        {
            response = await next();
        }
        catch (Exception exception)
        {
            failure = exception;
        }

        var entry = new AuditEntry(
            CommandType: request.GetType().FullName!,
            FriendlyName: auditableCommand.FriendlyName,
            Action: auditableCommand.Action,
            PayloadJson: Serialize(request),
            ResponseJson: failure is null ? Serialize(response) : SerializeFailure(failure),
            Succeeded: failure is null,
            Timestamp: DateTimeOffset.UtcNow);

        // Not the pipeline token: the audit must survive the caller aborting the request.
        await auditSink.WriteAsync(entry, CancellationToken.None);

        if (failure is not null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }

        return response;
    }

    private static string Serialize(object? value) =>
        JsonSerializer.Serialize(value, value?.GetType() ?? typeof(object), SerializerOptions);

    private static string SerializeFailure(Exception exception) =>
        JsonSerializer.Serialize(new { Error = exception.GetType().FullName, exception.Message });

    private static void SkipIgnoredProperties(JsonTypeInfo typeInfo)
    {
        foreach (var property in typeInfo.Properties)
        {
            if (property.AttributeProvider?.IsDefined(typeof(AuditTrailIgnoreAttribute), inherit: true) == true)
            {
                property.ShouldSerialize = static (_, _) => false;
            }
        }
    }
}
