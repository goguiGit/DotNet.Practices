# Template Method

Template Method en el dominio común de los de comportamiento (fichaje y ausencias). Explicación para humanos en `README.md`; aquí solo decisiones.

- Esqueleto en `Abstracts/PayableHoursCalculator.Calculate`; convenios en `Agreements/`; `WorkedDay` en la raíz.
- `Calculate` no es `virtual` a propósito. No abrirlo: si un convenio necesita otra estructura, es otro calculador (Strategy), no una subclase.
- Tres clases de paso: `DailyLimit` abstracto (sin valor por defecto deliberadamente), `OvertimeRate` virtual con 1,5 y `Counts` hook que acepta todo. Mantener esa distinción al añadir pasos.
- Los pasos son datos y un predicado, no métodos con cálculo. No añadir `protected abstract decimal RegularHours(...)`: dejaría a las subclases reescribir la lógica que el esqueleto fija.
- Dinero en `decimal` y los tests comparan con literales `m`.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/TemplateMethod/`.
