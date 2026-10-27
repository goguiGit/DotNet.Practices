# DuplicateKey

Clave única violada (`SqlException.Number` 2627 o 2601) tratada como resultado de negocio. Explicación en `README.md`.

- `ClockInRepository.InsertAsync` inserta sin comprobar antes: la restricción `UNIQUE (EmployeeId, Day)` es la fuente de verdad y el `catch ... when` solo traduce 2627/2601 a `ClockInResult.AlreadyExists`. Cualquier otro número sube.
- 2627 (constraint) y 2601 (índice único) se tratan igual a propósito.
- Parámetros con `SqlDbType` explícito; `DateOnly` va como `SqlDbType.Date` (soportado por SqlClient desde 5.1).
- El esquema de `ClockIns` vive en el test, no aquí.

Tests en `tests/DotNet.Practices.SqlErrors.Tests/DuplicateKey/`.
