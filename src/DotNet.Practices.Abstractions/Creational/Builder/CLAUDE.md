# Builder

Builder en el dominio común de los creacionales (logística). Explicación para humanos en `README.md`; aquí solo decisiones.

- Builders concretos en `Builders/`, contrato en `Interfaces/`. `Shipment`, `ShipmentDirector` y `TransportMode` en la raíz. No hay `Abstracts/`: no hay clase base con código.
- `TransportMode` repite el enum de `AbstractFactory/` a propósito: cada ejemplo es autocontenido. No compartir tipos entre ejemplos.
- `Build()` está en cada builder concreto, no en `IShipmentBuilder`: los productos son de tipos distintos (`Shipment` y `string`). No añadir un genérico `TProduct` a la interfaz; el porqué está en el README.
- Los pasos devuelven `IShipmentBuilder` para que el director encadene; el cliente conserva su referencia concreta para llamar a `Build()`.
- Toda receta del director empieza por `Reset()`. Hay un test que lo comprueba; si quitas el `Reset()`, se pone en rojo.
- `ShippingLabelBuilder` existe para que un test demuestre "mismos pasos, otro producto". Sin él, el patrón no se distingue de una clase fluida.

Tests en `tests/DotNet.Practices.Abstractions.Tests/Creational/Builder/`.
