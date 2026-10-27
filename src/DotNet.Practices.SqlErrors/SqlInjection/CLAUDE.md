# SqlInjection

Consulta parametrizada frente a concatenación. Explicación en `README.md`.

- `EmployeeDirectory.FindByNameAsync` usa `@name` con `SqlDbType.NVarChar, 100` (tipo y tamaño de la columna). No cambiar a `AddWithValue`.
- La consulta concatenada existe **solo** en `EmployeeDirectoryTests.ConcatenatedQuery`, para demostrar la fuga (`' OR 1 = 1 --` devuelve todo) y el error 102 ("Incorrect syntax near 'Brien'", no el 105 que uno esperaría) con "O'Brien". No moverla a `src/`; si Semgrep la marca, es a propósito.
- Igualdad exacta a propósito. Si alguien pasa a `LIKE`, hay que escapar `%`, `_` y `[` del usuario.
- Los nombres de base de datos que interpola `SqlServerTestDatabase` (`CREATE DATABASE [{name}]`) son GUIDs generados por el propio test, no entrada externa.

Tests en `tests/DotNet.Practices.SqlErrors.Tests/SqlInjection/`.
