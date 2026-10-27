# DotNet.Practices

Proyecto personal de prácticas .NET. Remoto GitHub vía alias SSH `github-personal` (clave en el ssh-agent de Windows; el repo usa `core.sshCommand` apuntando al OpenSSH de Windows).

## Estructura
- `src/` librerías y el proyecto `DotNet.Practices.Benchmarks` (BenchmarkDotNet, ejecutar en Release), `tests/` proyectos NUnit. Solución `DotNet.Practices.slnx`. Cinco librerías: `DotNet.Practices.Patterns` (los 23 patrones del catálogo, una carpeta por categoría), `DotNet.Practices.Abstractions` (ejemplos de abstracción: clases base, interfaces, un componente real sobre `IChatClient`), `DotNet.Practices.Mediator` (uso de MediatR al estilo Woffu), `DotNet.Practices.DynamicMapping` (mapper dinámico de property bags con planes compilados por LLM) y `DotNet.Practices.ScriptedMapping` (el enfoque original con Roslyn scripting, reconstruido para comparar). Cada una con su propio `CLAUDE.md` y `README.md`.
- Un ejemplo por carpeta (`Veterinary/`, `Translation/`, `Mapping/` en Abstractions; `Creational/FactoryMethod/`, `Structural/Composite/`, `Behavioral/Strategy/`... en Patterns), con su `CLAUDE.md` (decisiones, para el agente) y, en los patrones del catálogo, un `README.md` (explicación y diagrama, para humanos). Los patrones del catálogo cuelgan de una carpeta por categoría (`Creational/`, `Structural/`, `Behavioral/`), cada una con un `README.md` comparativo. El README raíz solo indexa. Interfaces en `Interfaces/`, clases base abstractas en `Abstracts/`, implementaciones agrupadas por plural (`Notifiers/`, `Animals/`, `Creators/`). Tests en la carpeta espejo.
- `Directory.Build.props`: TargetFramework, LangVersion, ImplicitUsings, Nullable. No repetirlos en los csproj.
- `Directory.Packages.props`: Central Package Management. `PackageReference` sin `Version`; la versión va ahí.

## Código
- Clases, métodos y nombres de test en inglés. Comentarios en inglés y solo cuando sean imprescindibles: un comentario es un code smell por defecto (ver `.claude/rules/comments.md`).
- Documentación para humanos (`README.md` de cada patrón) en español. Los identificadores de código citados se mantienen en inglés.
- Toda decisión documentada (README o CLAUDE.md) lleva el **porqué**: el problema que la motivó, la alternativa descartada y qué se gana. Una decisión sin porqué no se da por documentada.
- Todo `README.md` tiene **dos documentaciones**: primero la no técnica ("Qué hace, en términos de negocio", para gente de negocio, RRHH o soporte) y después la técnica. Detalle y prueba de fuego en `.claude/rules/documentation.md`.
- Nullable habilitado en todo el repo.
- CRLF en todos los ficheros.

## Testing
- TDD: test en rojo → implementar → verificar. Sin código de producción sin test previo.
- NUnit con `Assert.That(..., Is...)`. Un comportamiento y **un solo `Assert.That` por test**; sin `Assert.Multiple`. Para comprobar la forma de un objeto, compara el record completo con el esperado (`Is.EqualTo(new X(...) { ... })`); para varios casos, `[TestCase]`. Nombre `Clase_Metodo_Resultado`.
- `dotnet test DotNet.Practices.slnx` antes de commitear.

## Agentes (`.claude/agents/`)
**Generados**: `.claude/rules/*` y `.claude/agents/*` salen de `agent-kit/source/` con `dotnet run --project agent-kit/src/DotNet.Practices.AgentKit -- <repo>`; no se editan a mano (ver `agent-kit/CLAUDE.md`). Lo mismo produce `AGENTS.md`, `.cursor/rules/` y `.github/copilot-instructions.md` para otras herramientas. Inventario, papeles y origen en `.claude/README.md`. En corto: `researcher` (solo lectura), `implementer` (tarea cerrada con TDD), `test-writer` (solo el test en rojo), `tester` (verifica, no arregla), `dotnet-reviewer` (diff contra reglas), `dotnet-architect-reviewer` (revisión profunda; sus hallazgos se aplican con TDD en commits por tipo), `doc-writer` (README en dos partes y CLAUDE.md), `concept-explainer`. Los revisores, el investigador y el verificador no pueden escribir.

## Mapa de contexto
Cargar solo lo que la tarea necesita:

| Tarea | Leer |
|---|---|
| Escribir código C# | `.claude/rules/dotnet-standards.md`, `naming.md`, `comments.md`, `implementation.md` y el `CLAUDE.md` de la carpeta |
| Documentar | `.claude/rules/documentation.md` y el `README.md` vecino como modelo |
| Cambiar una regla o un agente | `agent-kit/source/` (fuente), luego regenerar; nunca `.claude/` directamente |
| Tocar un patrón | `src/DotNet.Practices.Patterns/CLAUDE.md`, `src/DotNet.Practices.Patterns/<Categoría>/README.md` y el `CLAUDE.md` del patrón |
| Mediator, DynamicMapping, ScriptedMapping | El `CLAUDE.md` y `README.md` del proyecto |
| Tests de integración, secrets | `.claude/rules/implementation.md` (seguridad al verificar) y `src/DotNet.Practices.Abstractions/Translation/CLAUDE.md` (user-secrets) |
## Git
- Conventional commits: feat:, fix:, chore:, refactor:, test:, docs:.
- No force push.
