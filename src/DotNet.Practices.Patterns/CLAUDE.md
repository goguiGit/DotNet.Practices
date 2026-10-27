# DotNet.Practices.Patterns

Catálogo de los 23 patrones del libro, una carpeta por categoría (`Creational/`, `Structural/`, `Behavioral/`) y una por patrón. Explicación para humanos en `README.md` y en el `README.md` de cada categoría y patrón; aquí solo decisiones de proyecto.

- Namespace = ruta: `DotNet.Practices.Patterns.<Categoría>.<Patrón>[.Subcarpeta]`. Tests en `tests/DotNet.Practices.Patterns.Tests/` con la misma ruta y namespace `DotNet.Practices.Patterns.Tests...`.
- Cada patrón es autocontenido. Tipos con el mismo nombre en varios patrones (`TransportMode`, `Document`, `LeaveRequest`) son deliberados; no extraer un modelo común.
- Dominio compartido por categoría: logística (creacionales), documentos (estructurales), fichaje y ausencias (comportamiento). Composite, Strategy y Chain of Responsibility son anteriores y conservan el suyo; no "alinearlos".
- Al añadir o tocar un patrón: README en dos partes, `CLAUDE.md`, fila en el `README.md` de la categoría y en la tabla del README raíz del repo. Mismo commit.
- La librería no tiene dependencias de paquete. `Microsoft.Extensions.DependencyInjection` está solo en el proyecto de tests (lo usa `Creational/Singleton`); no añadir extensiones `AddX()` aquí para evitarlo.
- Textos con importes o fechas usan `FormattableString.Invariant` o `yyyy-MM-dd`: los tests no deben depender de la cultura de la máquina.
- Tests sin mocks: los objetos reales exponen lo mínimo para observarlos (`Sent`, `Loads`, `Trace`). Si un test pide una librería de mocks, el ejemplo está mal planteado.

Las decisiones de cada patrón están en su propio `CLAUDE.md`.
