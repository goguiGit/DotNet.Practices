# TransientFaults

Errores transitorios de Azure SQL (40613, 40501, 49918...) reintentados con espera exponencial, a mano y con el reintento integrado de SqlClient. Explicación en `README.md`.

- `TransientSqlErrors.Numbers` es la única lista; `LeaveBalanceReader` la usa en el filtro `when` y `BuiltInRetryLeaveBalanceReader` la pasa en `SqlRetryLogicOption.TransientErrors`. No añadir 1205 ni -2: tienen su propio ejemplo.
- El reintento integrado funciona en SqlClient 7.1 sin ningún `AppContext` switch (las 3.x y 4.x pedían `Switch.Microsoft.Data.SqlClient.EnableRetryLogic`). Se asigna por comando con `SqlCommand.RetryLogicProvider`; `SqlConnection.RetryLogicProvider` solo cubre `Open`.
- Trampa: al agotar `NumberOfTries` el driver lanza `AggregateException` con una `SqlException` por intento, no la última `SqlException`. El test lo fija así a propósito.
- `SqlRetryLogicOption` valida rangos: `NumberOfTries` 1..60, `DeltaTime` ≤ 120 s, `MaxTimeInterval` ≤ 120 s.
- Simulación: el procedimiento `GetDaysLeft` del test (`TransientFaults/Outage.cs`) hace `RAISERROR(40613, 16, 1, 2, 'db', 3, 'srv', 5, 'trace')` mientras `Outages.FailuresLeft > 0`. Los `%.*ls` del mensaje consumen dos argumentos (longitud y texto); sin ellos RAISERROR falla con 2786. El no transitorio es `RAISERROR('...', 16, 1)`, número 50000.
- `backoff` es parámetro porque los tests lo necesitan en milisegundos; en producción son segundos.

Tests en `tests/DotNet.Practices.SqlErrors.Tests/TransientFaults/`.
