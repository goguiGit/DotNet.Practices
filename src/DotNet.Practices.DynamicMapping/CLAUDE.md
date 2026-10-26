# DotNet.Practices.DynamicMapping

Mapper dinámico de property bags (esquemas declarativos tipo Rabbit: propiedades tipadas, lookups, validadores y expresiones `$Context.X` / `$Services.F(...)` con dependencias entre propiedades) desde una fuente externa (p. ej. una BD de RRHH). El dominio original era Active Directory, pero el problema es genérico.

## Carpetas

- `Models/`: records del esquema (`PropertyBag`, `PropertyDefinition`, `PropertyType`, `SourceSchema`, `SourceColumn`). Solo lo que afecta al mapeo; sin nombres multiidioma ni mensajes de error.
- `Validators/`: `PropertyValidators` y, cuando lleguen, los validadores ejecutables (`required`, `pattern`, `maxlength`, `futureDate`). Todo lo que valida un valor va aquí, no en `Models/`.
- `Schema/`: `PropertyBagReader`, el lector de `propertybags.json`.
- `Plans/`: `MappingPlan`, `PropertyMapping`, `MappingKind`, `PlanKey`.
- `Dependencies/`: `DependencyExtractor` (saca los `Context.X` de una expresión), `DependencyGraph` (dependencias internas vs referencias externas, orden de evaluación estable, `CyclicDependencyException`).
- Próximas: `Resolution/`, `Stores/`.

## Decisiones de diseño (no reabrir sin motivo)

- **Compilar, no interpretar.** El LLM es el compilador en tiempo de diseño: traduce una regla (código o lenguaje natural) a una expresión determinista en un mini-DSL cerrado y extrae sus dependencias. En runtime se ejecuta la expresión sin modelo. El LLM en runtime queda solo para propiedades `Dynamic` (texto libre, elección semántica), siempre validadas después.
- **Se cachea el plan, no los valores.** `MappingPlan` describe cómo se obtiene cada propiedad; depende del esquema, no del registro. Dos niveles: `ConcurrentDictionary<PlanKey, Lazy<Task<MappingPlan>>>` en memoria y un store persistente (`IMappingPlanStore`, primera implementación en fichero JSON revisable en PR).
- **`PlanKey`** = SHA-256 del canónico de (bag destino, esquema origen). Propiedades y columnas ordenadas por nombre: el orden no cambia la clave. Incluye tipo, lookup, relation, expresiones, `unique`, `readOnly`, `extends` y validadores. **Excluye** nombres multiidioma y mensajes de error. Sin TTL: el plan caduca cuando cambia la clave.
- `MappingKind`: `Expression` y `Lookup` corren sin modelo; `Dynamic` lo necesita en cada registro; `Unmapped` es para una persona.
- Records con validación en el constructor mediante redeclaración de la propiedad posicional (`public string Value { get; } = Validate(Value);`). Es el patrón del proyecto para records inmutables con invariantes.

## Lector (`Schema/PropertyBagReader`)

Tolera lo que traen los ficheros reales: coma final, `readonly`/`readOnly` (claves case-insensitive), `name` como string u objeto por idioma, `defaultValue` no string (se guarda su texto JSON). Tipo desconocido o `id` ausente lanzan `FormatException` con el nombre de la propiedad.

- `PropertyDefinition.Type` es `null` solo en bags con `extends`: sus propiedades sobreescriben una del bag base y heredan el tipo. La resolución de `extends` es un paso posterior y el bag base (`AdObjectAccessTypePropertyBag`) **no está en la config de `qaacc`**: habrá que traerlo de otra fuente o declararlo como fixture.
- Validadores aún no modelados y por tanto ignorados: `emailDomain`, `emailLocalValidator`, `multiEmail`, `noWhitespace`, `range`, `multiValue`.

## Lo que falta (orden previsto)

1. Resolución de `extends` (merge por `id`, el hijo sobreescribe).
2. `ExpressionResolver` para el mini-DSL (interpolación, `Context.X`, catálogo cerrado de `Services.*` con stubs).
3. `MappingPlanner` con L1/L2 y tests de concurrencia (diez llamadas en frío, una compilación).
4. `RuleCompiler` sobre `IChatClient`. Usa el Azure OpenAI de user-secrets (`DotNet.Practices.Secrets`, mismas claves `OpenAI:*` que `Translation`). Tests de integración `[Category("Integration")]` con `Assert.Ignore` si faltan.

## Tests

- `Fixtures/PropertyBags/*.propertybags.json`: copias reales de Rabbit (`AdminAdAccount`, `StandardAdAccount`, `SharedFolder`, `DistributionGroup`), se copian al output. `PropertyBagFixtures` las lee.
- **Una clase de tests por fixture** (`<Nombre>FixtureTests`). Cada test compara una propiedad completa con el record esperado (`Is.EqualTo(new PropertyDefinition(...) { ... })`): un assert, forma entera documentada. Si se añade un fixture, se añade su clase.
- `PropertyBagReaderTests` solo prueba el comportamiento genérico del lector con JSON inline.
- Carpeta espejo del código: `Schema/`, `Plans/`, y las que vengan.
