# Bridge

Bridge en el dominio común de los estructurales (documentos). Explicación para humanos en `README.md`; aquí solo decisiones.

- Abstracción en `Abstracts/Document`, refinadas en `Documents/`, implementor en `Interfaces/IDocumentFormat`, implementaciones en `Formats/`.
- `IDocumentFormat` solo tiene primitivas (`Title`, `Line`). No añadir métodos por tipo de documento (`RenderPayslip`): rompe la independencia de los dos ejes.
- El formato entra por constructor primario y se expone como `Format` protegido. No pasarlo como parámetro de `Render()`: eso lo convierte en Strategy.
- Importes con `FormattableString.Invariant` para que los tests no dependan de la cultura de la máquina.
- `Document` se repite en otros estructurales: cada ejemplo es autocontenido. No compartir tipos entre ejemplos.

Tests en `tests/DotNet.Practices.Patterns.Tests/Structural/Bridge/`.
