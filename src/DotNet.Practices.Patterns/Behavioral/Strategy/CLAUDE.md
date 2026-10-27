# Strategy

Ejemplo del patrón Strategy con fan-out multi-tenant: cada canal es un `INotifier` intercambiable y cada compañía elige cuáles usa.

- `Interfaces/INotifier`: `Channel` + `SendAsync`. Una implementación por canal en `Notifiers/` (`EmailNotifier`, `PushNotifier`).
- `Company`: record con los canales habilitados. Es la configuración por tenant; el dispatcher deduplica, así que repetir un canal no envía dos veces.
- `NotificationDispatcher`: recibe todos los notifiers (en DI sería `IEnumerable<INotifier>`), los indexa por canal y envía en paralelo por cada canal habilitado de la compañía.

Decisiones ya tomadas:
- Un notifier por canal; duplicados lanzan en el constructor.
- Canal habilitado sin notifier registrado lanza `InvalidOperationException`: es un error de configuración, no se ignora en silencio.
- Añadir un canal = valor en `NotificationChannel` + clase `INotifier`. El dispatcher no se toca (open/closed).
- Los notifiers concretos guardan en memoria lo enviado (`Sent`) para que los tests verifiquen sin mocks. Es un atajo del ejemplo, marcado en código.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/Strategy/`.

Interfaces en `Interfaces/`, implementaciones de `INotifier` en `Notifiers/`. Namespace = ruta.
