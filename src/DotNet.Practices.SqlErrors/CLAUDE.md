# DotNet.Practices.SqlErrors

Errores comunes de SQL Server reproducidos contra una base real, una carpeta por error. Explicación para humanos en `README.md` y en el de cada carpeta; aquí solo decisiones de proyecto.

- Namespace = ruta: `DotNet.Practices.SqlErrors.<Ejemplo>`. Tests en `tests/DotNet.Practices.SqlErrors.Tests/` con la misma ruta.
- Único paquete: `Microsoft.Data.SqlClient` (más `System.Security.Cryptography.Pkcs` pinneado por un aviso de NuGet audit; quitar el pin cuando SqlClient suba su dependencia). Sin ORM a propósito: el ejemplo enseña la `SqlException`, no la librería que la envuelve.
- Los tests son todos `[Category("Integration")]`. Usan `Infrastructure/SqlServerTestDatabase`: crea una base `DotNetPractices_SqlErrors_<guid>` con el esquema del test y la borra en `Dispose`. Si el servidor no responde, `Assert.Ignore` con el motivo.
- Destino por defecto `Server=localhost` con autenticación integrada (la instancia local de desarrollo). `DOTNET_PRACTICES_SQLSERVER` lo cambia. No hay secretos en la cadena; si algún día los hay, pasan a user-secrets como en Abstractions.
- El esquema SQL de cada ejemplo vive en su test, no en la librería: la librería trata errores, no gestiona esquemas.
- Cada ejemplo distingue el error por `SqlException.Number` en un filtro `when`, nunca por el texto del mensaje ni con `catch (Exception)`.
- Al añadir un error: carpeta con README en dos partes y `CLAUDE.md`, fila en la tabla de este README y mención en el README raíz. Mismo commit.
