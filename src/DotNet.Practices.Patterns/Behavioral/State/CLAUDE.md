# State

State en el dominio común de los de comportamiento (fichaje y ausencias). Explicación para humanos en `README.md`; aquí solo decisiones.

- Contexto `LeaveRequest` en la raíz, contrato en `Interfaces/ILeaveState`, base en `Abstracts/LeaveState`, estados en `States/`.
- Las transiciones devuelven `ILeaveState`; el contexto hace `_state = _state.X()`. Los estados no reciben el contexto ni lo conocen. No añadir un setter público de estado.
- `LeaveState` prohíbe todo con `InvalidOperationException`; cada estado sobrescribe solo lo que admite. Una transición nueva es un `virtual` más en la base.
- `Status` es `string` (el `Name` del estado), no enum, para no invitar a un `switch` fuera. Si hay que persistir, el mapa nombre → clase va en un único sitio.
- Si aparece un `if (Status == ...)` en `LeaveRequest`, el patrón está roto.
- `LeaveRequest` se repite con el mismo nombre en otros ejemplos de la categoría: cada ejemplo es autocontenido. No compartir tipos entre ejemplos.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/State/`.
