# Veterinary

Ejemplo de abstracción: interfaz vs clase base vs interfaz de capacidad.

- `Interfaces/IAnimal`: contrato mínimo (`Greet`).
- `Animals/Animal`: clase base abstracta con `Greet` virtual por defecto; `Dog`, `Cat`, `Horse` lo sobreescriben.
- `Interfaces/ICageable`: interfaz marcadora. Solo la implementan las especies que caben en una jaula (`Dog`, `Cat`). `Horse` no, a propósito.
- `Cage`: acepta `ICageable`, no `IAnimal`. Meter un `Horse` es error de compilación, no de runtime.

Decisiones ya tomadas (no reabrir sin motivo):
- Capacidad de jaula como regla estática del tipo, no Visitor ni `switch` sobre tipos concretos.
- `Cage.Put` lanza `InvalidOperationException` al llenarse; es error del llamador, no resultado esperado.
- Si una regla depende del estado del animal (peso, agresividad), va en una política inyectada en `Cage`, no en `Animal`.

Tests en `tests/DotNet.Practices.Abstractions.Tests/Veterinary/`, mismo nombre de carpeta.

Interfaces en `Interfaces/`, la jerarquía de animales en `Animals/`. Namespace = ruta.

Cada ejemplo nuevo del proyecto va en su propia carpeta con namespace igual a la ruta y su carpeta espejo en tests.
