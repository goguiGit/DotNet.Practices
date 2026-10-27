# Observer

Observer en el dominio común de los de comportamiento (fichaje y ausencias). Explicación para humanos en `README.md`; aquí solo decisiones.

- Sujeto `LeaveApprovals` y `LeaveRequest` en la raíz; contrato en `Interfaces/IApprovalObserver`; observadores en `Observers/`.
- Interfaz explícita en vez de `event` a propósito, para que la lista y el recorrido se vean. No "modernizarlo" a `event`; el README explica cuándo usar cada cosa.
- El filtrado ("¿es mi solicitud?") lo hace el observador (`EmployeeMailbox`), no el sujeto. No añadir filtros a `Subscribe`.
- Notificación síncrona, en orden de suscripción, sin capturar excepciones. Es la misma semántica que `event`.
- `LeaveRequest` se repite con el mismo nombre en `Mediator/` y otros: cada ejemplo es autocontenido. No compartir tipos entre ejemplos.
- Fechas con `yyyy-MM-dd` en los mensajes para no depender de la cultura.

Tests en `tests/DotNet.Practices.Abstractions.Tests/Behavioral/Observer/`.
