# Mapping

Ejemplo del debate AutoMapper vs mapeo manual. Las dos implementaciones cumplen el mismo `IOrderMapper` y pasan los mismos tests (`TestFixtureSource`), así la comparación es justa.

- `Order` → `OrderDto`: colección anidada y tres campos calculados (`Total`, `LineCount`, `CreatedOn`). Es el caso donde AutoMapper deja de ser "gratis": cada campo calculado necesita un `ForCtorParam`.
- `Mappers/ManualOrderMapper`: un bucle, cero reflexión, el compilador verifica todo.
- `Mappers/AutoMapperOrderMapper`: envuelve `IMapper`. Guarda con `ThrowIfNull` porque `IMapper.Map(null)` devuelve `null` en silencio.
- `OrderProfile`: la configuración. `OrderProfileTests` llama a `AssertConfigurationIsValid()`; sin ese test, un campo nuevo en el DTO falla en runtime, no en compilación.

## Criterio

- Por defecto, mapeo manual: métodos de extensión `ToDto()` junto al DTO. Más código, pero visible, depurable y sin coste de arranque ni reflexión.
- AutoMapper solo si hay decenas de DTOs planos con nombres idénticos y el equipo acepta el test de configuración como obligatorio.
- Alternativa moderna: Mapperly (source generator, Apache 2.0). Genera el código manual por ti en compilación. No está en el ejemplo para no añadir una tercera dependencia.

## Licencia y seguridad

- AutoMapper 15+ es licencia comercial (Lucky Penny). 14.0.0 es la última MIT y es la que usa el ejemplo.
- 14.0.0 arrastra CVE-2026-32933 (DoS por recursión sin límite en mapas autorreferenciados), parcheada solo en 15.1.1+ / 16.1.1+. El aviso NU1903 está suprimido en `Directory.Build.props` porque ningún mapa del repo es autorreferenciado. En un proyecto real con MIT, la mitigación es `MaxDepth` en los mapas recursivos; o pagar la licencia; o dejar AutoMapper.
- Es el argumento central del debate: una dependencia de mapeo es una dependencia de licencia y de seguridad.

## Benchmark

`src/DotNet.Practices.Benchmarks`, BenchmarkDotNet con `[ShortRunJob]` y `[MemoryDiagnoser]`, 1/10/100 líneas. Ejecutar siempre en Release:

```
dotnet run -c Release --project src/DotNet.Practices.Benchmarks
```

Los números viven en el README; aquí solo la regla: nunca comparar sin baseline y nunca en Debug.

Tests en `tests/DotNet.Practices.Abstractions.Tests/Mapping/`.
