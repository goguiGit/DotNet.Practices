# Timeout

Consulta bloqueada que supera `CommandTimeout` (`SqlException.Number` -2) y cancelación del llamador. Explicación en `README.md`.

- `AttendanceReportQuery` recibe el `commandTimeout` en el constructor y lo pone en `SqlCommand.CommandTimeout` (segundos; el mínimo útil es 1). El -2 sube sin traducir: el llamador decide, aquí no se reintenta.
- Trampa: cancelar el token con el comando bloqueado produce `SqlException` ("A severe error occurred on the current command" o "Operation cancelled by user"), no `OperationCanceledException`. El filtro `catch (SqlException) when (cancellationToken.IsCancellationRequested)` la traduce. Mira el token, no el `Number`.
- El bloqueo en los tests es `SELECT 1 FROM ClockIns WITH (XLOCK, HOLDLOCK)` en una transacción que no se cierra; `UPDATE ... SET Day = Day` no sirvió para bloquear la lectura. El `SqlConnection` del bloqueo se devuelve al test para que su `Dispose` haga rollback; aun así `SqlServerTestDatabase.Dispose` mata lo que quede con `ROLLBACK IMMEDIATE`.
- El namespace `DotNet.Practices.SqlErrors.Timeout` convive con `System.Threading.Timeout` sin conflicto porque nadie usa el nombre corto `Timeout` en el código.

Tests en `tests/DotNet.Practices.SqlErrors.Tests/Timeout/`.
