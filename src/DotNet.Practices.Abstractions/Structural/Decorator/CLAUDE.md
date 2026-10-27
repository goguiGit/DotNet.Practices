# Decorator

Decorator en el dominio común de los estructurales (documentos). Explicación para humanos en `README.md`; aquí solo decisiones.

- Componente en `Interfaces/IDocument`, concreto `PlainDocument` en la raíz, base en `Abstracts/DocumentDecorator`, decoradores en `Decorators/`.
- `DocumentDecorator.Render()` es `virtual` y reenvía al interior; cada decorador hace `base.Render()` y añade. No saltarse la base llamando al interior directamente: si la interfaz crece, el reenvío debe vivir en un solo sitio.
- Los decoradores añaden al final a propósito, para que el orden de anidamiento se vea en el resultado. Hay un test que apila dos.
- No meter una lista de extras en `PlainDocument`: el componente no debe conocer sus decoradores.
- Misma estructura que `Proxy/`; la diferencia es la intención (añadir frente a controlar el acceso). Lo explica el README de la categoría.

Tests en `tests/DotNet.Practices.Abstractions.Tests/Structural/Decorator/`.
