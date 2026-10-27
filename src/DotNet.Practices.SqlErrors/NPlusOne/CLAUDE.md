# NPlusOne

Una consulta por cada fila, medida con un contador de consultas en el servidor. Explicación en `README.md`.

- `TeamAbsencesReport` (1 consulta, `LEFT JOIN` + agrupado en memoria) y `OneQueryPerEmployeeReport` (1 + N) devuelven el mismo `IReadOnlyList<EmployeeAbsences>`. La segunda se conserva a propósito para medirla; no "arreglarla".
- `EmployeeAbsences.Days` es texto `yyyy-MM-dd` unido por `", "` (`TeamAbsencesReport.Format`, invariante) para que el record compare por valor en los tests.
- El contador (`tests/.../NPlusOne/QueryCounter.cs`) crea una sesión de Extended Events con `sql_batch_completed` y `rpc_completed` filtrados por `database_id` y destino `event_counter` (síncrono), la lee y la borra. Cuenta idas a la base (lotes de texto y RPC como `sp_executesql`), no sentencias: `sql_statement_completed` no ve las sentencias de dentro de `sp_executesql`. Se descartó `sys.dm_exec_query_stats`: se publica con retraso y bajo carga devolvía 0. Necesita `ALTER ANY EVENT SESSION` y `VIEW SERVER STATE`.
- `DateOnly` se lee con `reader.GetFieldValue<DateOnly>()`; `GetDateTime` sobre una columna `date` también funciona pero obliga a convertir.

Tests en `tests/DotNet.Practices.SqlErrors.Tests/NPlusOne/`.
