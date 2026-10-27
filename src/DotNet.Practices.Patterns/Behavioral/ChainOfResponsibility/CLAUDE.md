# Chain of Responsibility

Ejemplo del patrón Chain of Responsibility en la forma que usa ASP.NET Core para los middleware.

- `RequestDelegate`: el eslabón. Un `Task` que procesa un `RequestContext`.
- `Interfaces/IMiddleware`: `InvokeAsync(context, next)`. Cada middleware decide si llama a `next` o corta la cadena.
- `PipelineBuilder`: `Use(middleware)` acumula y devuelve `this` (Fluent Interface) para encadenar `.Use(a).Use(b).Build(terminal)`; `Build` compone en orden inverso para que el primero registrado sea el más externo. Es lo que hace `IApplicationBuilder`.
- `Middlewares/`: `LoggingMiddleware` envuelve (antes y después de `next`), `AuthenticationMiddleware` corta con 401 si no hay usuario.
- `RequestContext`: `Path`, `User`, `StatusCode` (arranca en 404 como ASP.NET) y `Trace` para que los tests verifiquen el orden sin mocks.

## Recomendaciones

- El orden de `Use` es el orden de ejecución. Autenticación antes que lo que necesita usuario; logging/excepciones fuera del todo para que envuelvan a los demás.
- Cortar la cadena = no llamar a `next`. Fijar `StatusCode` y volver. Nunca lanzar para cortar.
- Llamar a `next` una sola vez. Dos llamadas ejecutan el resto de la cadena dos veces.
- Lo que va después de `await next(context)` corre en el camino de vuelta: ahí van métricas, logging de respuesta, limpieza.
- Middleware sin estado por petición. Si necesita estado, va en el contexto, no en campos de la clase: la misma instancia atiende todas las peticiones.
- Chain of Responsibility, no Decorator: el decorador siempre delega, el middleware puede decidir no hacerlo.
- Fluent Interface en el builder: cada `Use` devuelve el propio builder, nunca una copia; el método que cierra la cadena (`Build`) es el único que devuelve otro tipo. Igual que `IServiceCollection` o `IApplicationBuilder`.
- Mismo patrón en MediatR `IPipelineBehavior<TRequest,TResponse>` y en `DelegatingHandler` de `HttpClient`.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/ChainOfResponsibility/`.
