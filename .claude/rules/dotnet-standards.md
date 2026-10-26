# Estándares .NET

Basado en las convenciones oficiales de Microsoft (learn.microsoft.com/dotnet/csharp/fundamentals/coding-style).

## Tipos
- `record` para datos inmutables (DTOs, eventos, value objects). Clase mutable solo si hace falta estado.
- `readonly record struct` para value objects pequeños (ids, dinero).
- `sealed` por defecto en clases que no están pensadas para heredar.
- Composición antes que herencia. Herencia solo para un contrato real "es un" (ej. `Animal` → `Dog`).

## Nullability
- `Nullable` habilitado: no ignorar warnings CS86xx.
- `ArgumentNullException.ThrowIfNull(x)` para validar parámetros.
- No usar `!` (null-forgiving) salvo con un comentario que justifique por qué es seguro.

## Expresiones
- `switch` expression y pattern matching antes que cadenas de `if/else`.
- Expression-bodied members (`=>`) para una sola expresión.
- `var` cuando el tipo es evidente en la misma línea; tipo explícito si no.
- `is null` / `is not null`, no `== null`.

## Async
- Sufijo `Async` y `CancellationToken cancellationToken = default` en todo método async.
- Nunca `.Result` ni `.Wait()`.

## Colecciones y APIs
- Aceptar `IEnumerable<T>` / `IReadOnlyList<T>`, no exponer `List<T>` mutable.
- Excepciones solo para errores inesperados, no para flujo de negocio.

## Ficheros
- Un tipo público por fichero, nombre del fichero = nombre del tipo.
- File-scoped namespace (`namespace X;`).
- CRLF.
