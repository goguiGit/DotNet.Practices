# Singleton

Singleton en el dominio común de los creacionales (logística), en dos versiones: la clásica como contraejemplo y la de DI como la que se escribe hoy. Explicación para humanos en `README.md`; aquí solo decisiones.

- `DispatchCenter` es la versión clásica (`Lazy<T>`, constructor privado, `static Instance`). No implementa `ITrackingSequence` a propósito: si fuera inyectable dejaría de ser el contraejemplo.
- `TrackingSequence` es una clase normal; "una sola" lo decide `AddSingleton` en quien compone la aplicación. Aquí, el test.
- El paquete `Microsoft.Extensions.DependencyInjection` está solo en `DotNet.Practices.Abstractions.Tests`, no en la librería. No añadir una extensión `AddX()` a `Abstractions` para evitarlo.
- El test del contador de `DispatchCenter` compara dos llamadas consecutivas, no un valor absoluto: el contador es global al proceso y depende de los tests anteriores. Esa limitación es el argumento del README, no un bug.
- Ambas versiones usan `Interlocked.Increment`; no sustituir por `++`.

Tests en `tests/DotNet.Practices.Abstractions.Tests/Creational/Singleton/`.
