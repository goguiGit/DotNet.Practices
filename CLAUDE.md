# DotNet.Practices

Proyecto personal de prácticas .NET. Remoto GitHub vía alias SSH `github-personal` (clave en el ssh-agent de Windows; el repo usa `core.sshCommand` apuntando al OpenSSH de Windows).

## Estructura
- `src/` librerías, `tests/` proyectos NUnit. Solución `DotNet.Practices.slnx`.
- `Directory.Build.props`: TargetFramework, LangVersion, ImplicitUsings, Nullable. No repetirlos en los csproj.
- `Directory.Packages.props`: Central Package Management. `PackageReference` sin `Version`; la versión va ahí.

## Código
- Clases, métodos y nombres de test en inglés. Comentarios en inglés.
- Nullable habilitado en todo el repo.
- CRLF en todos los ficheros.

## Testing
- TDD: test en rojo → implementar → verificar. Sin código de producción sin test previo.
- NUnit con `Assert.That(..., Is...)`. Un comportamiento por test, nombre `Clase_Metodo_Resultado`.
- `dotnet test DotNet.Practices.slnx` antes de commitear.

## Git
- Conventional commits: feat:, fix:, chore:, refactor:, test:, docs:.
- No force push.
