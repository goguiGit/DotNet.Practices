# DotNet.Practices

Proyecto personal de prácticas .NET. Remoto GitHub vía alias SSH `github-personal` (clave en el ssh-agent de Windows; el repo usa `core.sshCommand` apuntando al OpenSSH de Windows).

## Estructura
- `src/` librerías y el proyecto `DotNet.Practices.Benchmarks` (BenchmarkDotNet, ejecutar en Release), `tests/` proyectos NUnit. Solución `DotNet.Practices.slnx`. Tres librerías: `DotNet.Practices.Abstractions` (ejemplos de patrones), `DotNet.Practices.Mediator` (uso de MediatR al estilo Woffu) y `DotNet.Practices.DynamicMapping` (mapper dinámico de property bags con planes compilados por LLM). Cada una con su propio `CLAUDE.md`.
- Un ejemplo por carpeta (`Veterinary/`, `Military/`, `Notifications/`, `RequestPipeline/`, `Translation/`, `Mapping/`, `Creational/FactoryMethod/`...), con su `CLAUDE.md` (decisiones, para el agente) y, en los patrones del catálogo, un `README.md` (explicación y diagrama, para humanos). Los patrones del catálogo cuelgan de una carpeta por categoría (`Creational/`, `Structural/`, `Behavioral/`), cada una con un `README.md` comparativo. El README raíz solo indexa. Interfaces en `Interfaces/`, clases base abstractas en `Abstracts/`, implementaciones agrupadas por plural (`Notifiers/`, `Animals/`, `Creators/`). Tests en la carpeta espejo.
- `Directory.Build.props`: TargetFramework, LangVersion, ImplicitUsings, Nullable. No repetirlos en los csproj.
- `Directory.Packages.props`: Central Package Management. `PackageReference` sin `Version`; la versión va ahí.

## Código
- Clases, métodos y nombres de test en inglés. Comentarios en inglés y solo cuando sean imprescindibles: un comentario es un code smell por defecto (ver `.claude/rules/comments.md`).
- Documentación para humanos (`README.md` de cada patrón) en español. Los identificadores de código citados se mantienen en inglés.
- Nullable habilitado en todo el repo.
- CRLF en todos los ficheros.

## Testing
- TDD: test en rojo → implementar → verificar. Sin código de producción sin test previo.
- NUnit con `Assert.That(..., Is...)`. Un comportamiento y **un solo `Assert.That` por test**; sin `Assert.Multiple`. Para comprobar la forma de un objeto, compara el record completo con el esperado (`Is.EqualTo(new X(...) { ... })`); para varios casos, `[TestCase]`. Nombre `Clase_Metodo_Resultado`.
- `dotnet test DotNet.Practices.slnx` antes de commitear.

## Agentes (`.claude/agents/`)
- `dotnet-reviewer`: revisión rápida de un diff contra `.claude/rules`. Tras implementar algo.
- `dotnet-architect-reviewer`: revisión profunda de un proyecto con skills externas (awesome-copilot) y locales (dotnet-skills). Al cerrar una pieza grande o antes de un push importante; sus hallazgos se aplican con TDD y en commits separados por tipo (`feat`/`fix`/`refactor`/`test`/`docs`).
- `test-writer`: escribe el test en rojo. `concept-explainer`: explica un concepto con tipos del repo.

## Git
- Conventional commits: feat:, fix:, chore:, refactor:, test:, docs:.
- No force push.
