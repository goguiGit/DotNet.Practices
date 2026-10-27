# Mediator

Mediator del libro en el dominio común de los de comportamiento (fichaje y ausencias). La versión con MediatR está en `src/DotNet.Practices.Mediator/`. Explicación para humanos en `README.md`; aquí solo decisiones.

- Mediador en `Interfaces/ILeaveDesk` y `LeaveDesk`; colegas en `Colleagues/`. `LeaveRequest` en la raíz.
- Interfaz tipada (`Request`, `Approve`), no `Notify(sender, string)`. Un evento nuevo es un método nuevo.
- `LeaveDesk` construye `Manager` y `TeamCalendar` en su constructor para que no exista una mesa a medio montar. Los `Employee` se crean fuera con la mesa como dependencia.
- `Employee` y `Manager` solo conocen `ILeaveDesk`. `TeamCalendar` no conoce a nadie. Si una prueba necesita que un colega llame a otro directamente, el patrón está roto.
- `LeaveRequest` lleva el nombre, no el objeto `Employee`; la mesa guarda el mapa solicitud → empleado para avisar al aprobar.
- Fechas formateadas con `yyyy-MM-dd` para que los mensajes no dependan de la cultura.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/Mediator/`.
