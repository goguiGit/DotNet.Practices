# Command

Command en el dominio común de los de comportamiento (fichaje y ausencias). Explicación para humanos en `README.md`; aquí solo decisiones.

- Contrato en `Interfaces/ICommand` (`Execute` y `Undo`, sin interfaz aparte para deshacer), comandos en `Commands/`, receptor `Timesheet` e invocador `CommandHistory` en la raíz.
- Los comandos no reciben parámetros en `Execute`/`Undo`: todo entra por el constructor primario. Si un comando necesita algo en el momento de ejecutar, es señal de que falta un campo.
- El estado para deshacer vive en el comando, no en `Timesheet`. No añadir histórico al receptor; eso es `Memento/`.
- `CommandHistory.Undo()` con la pila vacía no hace nada (`TryPop`). No cambiarlo por una excepción.
- `ClockEntry`, `ClockDirection` y `Timesheet` se repiten con el mismo nombre en otros ejemplos de la categoría: cada ejemplo es autocontenido. No compartir tipos entre ejemplos.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/Command/`.
