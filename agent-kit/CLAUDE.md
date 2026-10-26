# agent-kit

Fuente única de reglas y roles de agente del repo, y el generador .NET que produce `.claude/`, `AGENTS.md`, `.cursor/rules/` y `.github/copilot-instructions.md`. Explicación y porqués en `README.md`; aquí solo lo operativo.

- **Nunca editar los ficheros generados.** `.claude/rules/*`, `.claude/agents/*`, `AGENTS.md`, `.cursor/rules/*` y `.github/copilot-instructions.md` llevan un marcador en la primera línea útil. Se edita `agent-kit/source/` y se regenera: `dotnet run --project agent-kit/src/DotNet.Practices.AgentKit -- <repo>`. Fuente y salida se commitean juntas.
- `--check` devuelve 1 si hay deriva; lo corre `.github/workflows/agent-kit-check.yml` en Linux junto con los tests del kit. El workflow es manual, no generado: el generador solo escribe `.github/copilot-instructions.md`.
- Solución propia (`AgentKit.slnx`), fuera de `DotNet.Practices.slnx` a propósito: es herramienta, no ejemplo. Usa `Directory.Build.props` y `Directory.Packages.props` de la raíz como todo lo demás.
- Frontmatter neutral de un rol: `name`, `description`, `model` obligatorios; `tools` lista separada por comas; `readOnly: true` para los que no deben escribir. El lector lanza `FormatException` con el fichero y la clave que falta.
- Un generador por herramienta implementa `IGenerator`. Añadir una herramienta = una clase y sus tests; nada más cambia.
- Salida siempre CRLF y UTF-8 sin BOM, independiente de la plataforma, para que `--check` dé lo mismo en cualquier máquina.
- `.claude/README.md` y el `CLAUDE.md` raíz se mantienen a mano: son del repo, no de la fuente del kit.

Tests en `agent-kit/tests/DotNet.Practices.AgentKit.Tests/{Source,Generators,Output}/`.
