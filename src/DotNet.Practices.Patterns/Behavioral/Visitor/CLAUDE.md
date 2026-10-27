# Visitor

Visitor en el dominio común de los de comportamiento (fichaje y ausencias). Explicación para humanos en `README.md`; aquí solo decisiones.

- Contratos en `Interfaces/` (`ICalendarEntry`, `ICalendarVisitor<out TResult>`), elementos en `Entries/` como `record`, visitantes en `Visitors/`.
- `Accept` devuelve `TResult`; los visitantes no acumulan estado. No volver a `void Visit` con un campo `Result`.
- Una sobrecarga `Visit` por tipo concreto y ningún respaldo `Visit(ICalendarEntry)`. Añadir un tipo de apunte debe romper la compilación de todos los visitantes; es la garantía del patrón.
- `Accept` es siempre `visitor.Visit(this)`, idéntico en cada elemento. No intentar moverlo a una base: el despacho depende del tipo estático de `this`.
- El README incluye la alternativa con `switch` y pattern matching; cuando se dude, esa es la primera opción en C# moderno.
- `SummaryVisitor` interpola `decimal`; los tests usan horas enteras para no depender de la cultura.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/Visitor/`.
