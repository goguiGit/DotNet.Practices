# Memento

Memento en el dominio común de los de comportamiento (fichaje y ausencias). Explicación para humanos en `README.md`; aquí solo decisiones.

- Originador `LeaveRequestDraft` y cuidador `DraftHistory` en la raíz; interfaz vacía `Interfaces/IDraftSnapshot`. El memento real es `LeaveRequestDraft.Snapshot`, record privado anidado. No hacerlo público ni sacarlo a su propio fichero: perdería la opacidad.
- `Save()` copia la lista de días (`[.. _days]`). No guardar la referencia; hay un test que lo detecta.
- `Restore` hace un cast a `Snapshot`. Es el único `cast` aceptable aquí porque nadie más puede crear un `IDraftSnapshot` útil.
- `DraftHistory.Undo` con la pila vacía no hace nada (`TryPop`), igual que `CommandHistory` en `Command/`.
- Deshacer por foto aquí y por delta en `Command/` es deliberado; la comparación está en el README de ambos.

Tests en `tests/DotNet.Practices.Abstractions.Tests/Behavioral/Memento/`.
