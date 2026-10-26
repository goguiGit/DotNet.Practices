# DotNet.Practices.Mediator

Cómo usa Woffu MediatR, reducido a tres piezas de `Woffu.Library.MediatR`: Abstractions, Behaviors.Cache y Behaviors.AuditTrail. Es un ejemplo: no referencia los paquetes privados, replica la idea con dependencias públicas.

## Abstractions (`Abstractions/Messaging/`)
- `ICommand<TResponse>` e `IQuery<TResponse>` son `IRequest<TResponse>` con nombre. Separan escritura de lectura solo por tipo; MediatR no distingue.
- `ICommandHandler` / `IQueryHandler` son `IRequestHandler` con constraint. Los handlers implementan estos, nunca `IRequestHandler` a secas.
- En Woffu `TResponse` está acotado a `IResultHandler` (Woffu.Library.ErrorHandling). Aquí no, para no arrastrar esa librería.

## CachingBehavior (`Behaviors/Cache/`)
- Cachea la respuesta de cualquier request que implemente `ICacheRequest` (`CacheKey`, `CacheExpiration`). Si no lo implementa, pasa de largo.
- Expiración por defecto: 1 día. Woffu usa FusionCache sobre Redis con una `CacheDatabase` por request; aquí `IMemoryCache`.
- Solo tiene sentido en queries. Un command cacheado es un bug.

## AuditCommandBehavior (`Behaviors/AuditTrail/`)
- Para un `IAuditableCommand` (`FriendlyName`, `Action`) serializa payload y respuesta a JSON y escribe un `AuditEntry` en `IAuditSink`. Woffu publica un `AuditTrailIntegrationEvent` al event bus y añade usuario, compañía y `RootActionId`.
- `[AuditTrailIgnore]` en una propiedad la excluye del JSON a cualquier profundidad (campos sensibles o pesados). En Woffu también existen `IIgnorePayloadAuditTrail` e `IIgnoreResponseAuditTrail`.
- Si el handler lanza, se audita el fallo y se relanza con `ExceptionDispatchInfo` para no perder el stack.
- El sink se escribe con `CancellationToken.None`: la auditoría tiene que sobrevivir a que el cliente aborte la petición. No cambiar.

## Registro
- `services.AddMediatR(...)` + `services.AddMediatorBehaviors()`. El orden de registro de los behaviors es el orden de ejecución: auditoría fuera, caché dentro.
- MediatR pinneado a 12.5.0 en `Directory.Packages.props`: 13+ cambió a licencia comercial.

## Tests
- `Fixtures/TestMediator` monta un `ServiceCollection` real con los behaviors, un `InMemoryAuditSink` y handlers singleton para contar llamadas. Sin mocks.
- Tests en `tests/DotNet.Practices.Mediator.Tests/Behaviors/`.
