# DotNet.Practices.ScriptedMapping

Reimplementación mínima y fiel de cómo Rabbit evalúa property bags: expresiones `$` como C# script con Roslyn, `Context`/`Services` dinámicos y bucle scan-and-restart. Es el "antes" contra el que se compara `DynamicMapping`. Explicación, porqués y comparación en `README.md`; aquí solo decisiones de este código.

## Decisiones

- **Fidelidad antes que calidad.** Se replican a propósito comportamientos discutibles de Rabbit porque son el objeto de estudio: `'` → `"` sin escape, `RuntimeBinderException` tragada (la propiedad queda `null`), `ParseValue` que convierte `"123"` en `int`, orden O(n²). No "mejorar" nada aquí; las mejoras van a `DynamicMapping`.
- **Reutiliza `DynamicMapping.Models` y `PropertyBagReader`** vía referencia de proyecto, y `DependencyExtractor` por regex en lugar del análisis semántico de Roslyn de Rabbit. La detección de dependencias no es lo que se compara; la estrategia de evaluación sí.
- `ScriptServices` hereda de `DynamicObject` y despacha por nombre con reflexión y `OptionalParamBinding`, como `ScriptGlobalsServices` en Rabbit. Los métodos son públicos y tipados para poder testear la semántica sin pasar por `dynamic`.
- `GetFiveTwoOneIdValue`: contador base 36 desde 1, siempre en mayúsculas aunque `upper` sea `false`; con dos dígitos el nombre se reduce a un carácter. Secuencia `UniquePropertyBagPropertyId.{5}{inicial}{sufijo}`. Verificado contra `FiveTwoOneIdScriptService.cs` de Rabbit.
- `ISequenceTracker` en memoria; Rabbit lo respalda con base de datos y reserva global.
- `Combine` es `Path.Combine`: en Windows produce `\`. Los tests asumen Windows, igual que Rabbit.
- Fixtures enlazados desde `DotNet.Practices.DynamicMapping.Tests/Fixtures/PropertyBags` con `Link`: un solo origen de verdad para los JSON reales.

## Lo que no está

Registro de tipos y fusión de `extends`, validadores, backfeed, `Relations`, `Variables`, `.csx` de servicios extra. Ver la tabla "Qué replica y qué deja fuera" del README.

Tests en `tests/DotNet.Practices.ScriptedMapping.Tests/{Scripting,Services,Evaluation}/`.
