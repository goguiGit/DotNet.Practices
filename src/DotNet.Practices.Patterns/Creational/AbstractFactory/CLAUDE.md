# AbstractFactory

Abstract Factory en el dominio común de los creacionales (logística). Explicación para humanos en `README.md`; aquí solo decisiones.

- Productos agrupados por abstracción: `Transports/` y `Packagings/`. Fábricas en `Factories/`. Contratos en `Interfaces/`. No hay `Abstracts/` porque no hay clase base con código.
- `ITransport`/`Truck`/`Ship` repiten nombres de `FactoryMethod/` a propósito: cada ejemplo es autocontenido y los namespaces los separan. No compartir tipos entre ejemplos.
- `IPackaging.CanBeLoadedOn(ITransport)` existe para que un test demuestre la garantía del patrón (compatibilidad dentro de la familia). Sin eso, Abstract Factory no se distingue de dos Factory Method.
- `LogisticsFactories.For(mode)` es el único `switch` sobre concretos, y está fuera del cliente. Representa el registro de DI; no lo muevas dentro de `ShipmentPlanner`.
- Si un test necesita un `new Truck()` dentro del cliente, el patrón está roto.

Tests en `tests/DotNet.Practices.Patterns.Tests/Creational/AbstractFactory/`.
