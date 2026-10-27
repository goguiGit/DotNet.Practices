# Adapter

Adapter en el dominio común de los estructurales (documentos). Explicación para humanos en `README.md`; aquí solo decisiones.

- Target en `Interfaces/IDocumentRenderer`, adaptee en `Adaptees/` (simula una librería ajena, no la toques para "arreglar" su API), adaptador en `Adapters/`, implementación nativa en `Renderers/`. `Document` en la raíz.
- Adapter de objeto: el motor se inyecta por constructor. No heredar de `LegacyPdfEngine`.
- `LegacyPdfEngine` guarda estado entre `Open` y `Close` a propósito; el adaptador debe abrir y cerrar en cada `Render`. Hay un test que lo comprueba.
- `Document` se repite en otros estructurales con el mismo nombre: cada ejemplo es autocontenido. No compartir tipos entre ejemplos.

Tests en `tests/DotNet.Practices.Patterns.Tests/Structural/Adapter/`.
