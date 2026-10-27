# Flyweight

Flyweight en el dominio común de los estructurales (documentos). Explicación para humanos en `README.md`; aquí solo decisiones.

- Cuatro tipos en la raíz, sin subcarpetas: no hay abstracción con varias implementaciones.
- `TextStyle` es un `record` inmutable y es también la clave del diccionario de la fábrica (`Dictionary<TextStyle, TextStyle>`). No sustituir la clave por un `string` concatenado.
- `DocumentPage` nunca hace `new TextStyle`; siempre `styles.Get(...)`. Si un test necesita construir un `TextStyle` a mano es solo para comparar por valor.
- El test de ahorro cuenta instancias (`Count`), no memoria. No añadir `GC.GetTotalMemory` a los tests; eso va a `DotNet.Practices.Benchmarks` si algún día se mide.

Tests en `tests/DotNet.Practices.Patterns.Tests/Structural/Flyweight/`.
