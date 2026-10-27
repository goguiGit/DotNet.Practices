# Prototype

Prototype en el dominio común de los creacionales (logística). Explicación para humanos en `README.md`; aquí solo decisiones.

- Base abstracta en `Abstracts/ShipmentTemplate`, plantillas concretas en `Templates/`, registro `ShipmentTemplates` en la raíz. No hay `Interfaces/`: el contrato lleva código (el constructor de copia), así que es clase abstracta.
- Copia profunda por constructor de copia encadenado (hija → base). No sustituir por `MemberwiseClone()`: la lista `Stops` quedaría compartida y hay un test que lo detecta.
- Los constructores de copia de las hijas son privados: solo `Clone()` los usa. El de la base es `protected`.
- `Stops` es `ICollection<string>` mutable a propósito: es lo que demuestra la diferencia entre copia profunda y `with`. No cambiarlo a `IReadOnlyList`.
- Sin `ICloneable` ni `record`; el porqué está en el README.
- `ShipmentTemplates.Create` devuelve `Clone()`, nunca la plantilla registrada. Un nombre desconocido lanza `KeyNotFoundException` (la del diccionario, sin envolver).

Tests en `tests/DotNet.Practices.Abstractions.Tests/Creational/Prototype/`.
