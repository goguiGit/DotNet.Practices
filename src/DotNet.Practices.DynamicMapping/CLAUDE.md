# DotNet.Practices.DynamicMapping

Mapper dinámico de property bags (esquemas declarativos tipo Rabbit: propiedades tipadas, lookups, validadores y expresiones `$Context.X` / `$Services.F(...)` con dependencias entre propiedades) desde una fuente externa (p. ej. una BD de RRHH). El dominio original era Active Directory, pero el problema es genérico. Explicación para humanos con diagramas en `README.md`; aquí solo decisiones.

## Carpetas

- `Models/`: records del esquema (`PropertyBag`, `PropertyDefinition`, `PropertyType`, `SourceSchema`, `SourceColumn`). Solo lo que afecta al mapeo; sin nombres multiidioma ni mensajes de error.
- `Validators/`: `PropertyValidators` (declaración) y `PropertyValidator.Validate(property, value)` (ejecución: tipo, `required`, `pattern`, `maxlength`, `futureDate`; devuelve la lista de violaciones). Fechas solo ISO (`DateOnly.TryParseExact`; `DateTime.TryParse` aceptaba `"2030"` como hora). El regex del `pattern` lleva `matchTimeout`: un patrón catastrófico del JSON externo falla el valor, no cuelga el proceso. Todo lo que valida un valor va aquí, no en `Models/`.
- `Schema/`: `PropertyBagReader`, el lector de `propertybags.json`.
- `Plans/`: `MappingPlan`, `PropertyMapping`, `MappingKind`, `PlanKey`.
- `Dependencies/`: `DependencyExtractor` (saca los `Context.X` de una expresión), `DependencyGraph` (dependencias internas vs referencias externas, orden de evaluación estable, `CyclicDependencyException`).
- `Resolution/`: `MappingContext` (Owner = registro externo, Inputs = lo que teclea el solicitante, Options = contenedores y lookups), `ResolvedBag`, `Interfaces/IPropertyBagResolver` y `LlmPropertyBagResolver`, la línea base en la que el modelo evalúa todas las propiedades en una llamada.
- Próximas: `Stores/`, el compilador de planes.

## Decisiones de diseño (no reabrir sin motivo)

- **Compilar, no interpretar.** El LLM es el compilador en tiempo de diseño: traduce una regla (código o lenguaje natural) a una expresión determinista en un mini-DSL cerrado y extrae sus dependencias. En runtime se ejecuta la expresión sin modelo. El LLM en runtime queda solo para propiedades `Dynamic` (texto libre, elección semántica), siempre validadas después.
- **Se cachea el plan, no los valores.** `MappingPlan` describe cómo se obtiene cada propiedad; depende del esquema, no del registro. Dos niveles: `ConcurrentDictionary<PlanKey, Lazy<Task<MappingPlan>>>` en memoria y un store persistente (`IMappingPlanStore`, primera implementación en fichero JSON revisable en PR).
- **`PlanKey`** = SHA-256 del canónico **en JSON** de (bag destino, esquema origen, versión del compilador). JSON y no campos unidos por `|`: así `null` y `""` difieren y un separador dentro de un valor no puede colisionar. Propiedades, columnas y `extends` ordenados: el orden no cambia la clave. Incluye tipo, lookup, relation, expresiones, `unique`, `readOnly` y validadores. **Excluye** nombres multiidioma y mensajes de error. Sin TTL: el plan caduca cuando cambia la clave. `For_KnownInputs_MatchesGoldenHash` fija el canónico: si cambia, cambia el hash dorado a propósito y se asume que invalida todos los planes guardados.
- La clave se calcula **después** de resolver `extends`: con el bag sin fusionar solo entra el nombre del base, y un cambio en el base no invalidaría a los hijos.
- `MappingKind`: `Expression` y `Lookup` corren sin modelo; `Dynamic` lo necesita en cada registro; `Unmapped` es para una persona.
- Records `sealed` con validación en el constructor mediante redeclaración de la propiedad posicional (`public string Value { get; } = Validate(Value);`). Trampa: `with` copia campos sin reejecutar inicializadores, así que toda propiedad que participe en un invariante se redeclara **get-only** (`Kind` y `Expression` en `PropertyMapping`); solo las libres admiten `init`. Y no usar structs para esto: `default(T)` se salta la validación.
- `PropertyBag` y `MappingPlan` rechazan ids duplicados en el constructor; `CyclicDependencyException.PropertyIds` lista solo los miembros del ciclo, no los que dependen de él.

## Lector (`Schema/PropertyBagReader`)

Tolera lo que traen los ficheros reales: coma final, `readonly`/`readOnly` (claves case-insensitive), `name` como string u objeto por idioma, `defaultValue` no string (se guarda su texto JSON). Tipo desconocido o `id` ausente lanzan `FormatException` con el nombre de la propiedad.

- `PropertyDefinition.Type` es `null` solo en bags con `extends`: sus propiedades sobreescriben una del bag base y heredan el tipo. La resolución de `extends` es un paso posterior y el bag base (`AdObjectAccessTypePropertyBag`) **no está en la config de `qaacc`**: habrá que traerlo de otra fuente o declararlo como fixture.
- Validadores aún no modelados y por tanto ignorados: `emailDomain`, `emailLocalValidator`, `multiEmail`, `noWhitespace`, `range`, `multiValue`.

## Línea base con LLM (`LlmPropertyBagResolver`)

- Prompt: system con las reglas (orden de evaluación, inputs nunca inventados, solo JSON) y user con Owner, Inputs, Options, la semántica de `Services.*` **en lenguaje natural** y las propiedades en orden de evaluación con expresión, validadores y dependencias. La semántica de `Services.*` es la real de Rabbit (ver `ScriptedMapping`).
- Resultado observado con gpt-4o sobre los 4 fixtures: todas las propiedades válidas, pero `SamAccountName` salió `lovel-adm-26` (sin las dos letras del nombre) en un bag y `lovelada26` en otro para la misma regla. Es la evidencia de por qué el plan compilado y determinista es el camino de runtime, y esta clase solo la línea base.
- Sin `Inputs`, las propiedades obligatorias sin expresión (`Name`, `PrimarySmtpAddressLocalPart`) quedan nulas y el test falla: correcto, no deben inventarse.

## Riesgos abiertos

- La semántica de `Services.*` del prompt ya es la real (verificada en Rabbit y reproducida en `ScriptedMapping`), pero sigue siendo texto: el `RuleCompiler` debe usar `ScriptedMapping.Services` como catálogo de funciones del mini-DSL y verificar con ejemplos dorados (contexto → valores reales).
- `Lookup` y `Relation` no se validan en `PropertyValidator` contra el catálogo; lo hace solo el test de integración.
- El L1 previsto con `Lazy<Task<MappingPlan>>` cachea tareas fallidas: retirar la entrada al fallar.
- Validadores `emailDomain`, `multiValue`, `range`, etc. siguen ignorados; `SecondarySmtpAddresses` se trata como texto simple.

## Lo que falta (orden previsto)

1. Resolución de `extends` (merge por `id`, el hijo sobreescribe).
2. `ExpressionResolver` para el mini-DSL (interpolación, `Context.X`, catálogo cerrado de `Services.*` con stubs).
3. `MappingPlanner` con L1/L2 y tests de concurrencia (diez llamadas en frío, una compilación).
4. `RuleCompiler` sobre `IChatClient`. Usa el Azure OpenAI de user-secrets (`DotNet.Practices.Secrets`, mismas claves `OpenAI:*` que `Translation`). Tests de integración `[Category("Integration")]` con `Assert.Ignore` si faltan.

## Tests

- `Fixtures/PropertyBags/*.propertybags.json`: copias reales de Rabbit (`AdminAdAccount`, `StandardAdAccount`, `SharedFolder`, `DistributionGroup`), se copian al output. `PropertyBagFixtures` las lee.
- **Una clase de tests por fixture** (`<Nombre>FixtureTests`). Cada test compara una propiedad completa con el record esperado (`Is.EqualTo(new PropertyDefinition(...) { ... })`): un assert, forma entera documentada. Si se añade un fixture, se añade su clase.
- `PropertyBagReaderTests` solo prueba el comportamiento genérico del lector con JSON inline.
- `PropertyBagOverviewTests`: un caso por bag que escribe en el log la tabla de propiedades, dependencias y orden; asserta que no hay ciclos.
- `LlmPropertyBagResolverIntegrationTests` (`[Category("Integration")]`): el test general por tipo. Un caso por bag sin `extends`, resuelve todas las propiedades con el Azure OpenAI de user-secrets, escribe valores y tokens en el log y asserta, con un solo `Assert.That` sobre la lista de violaciones: validadores declarados, lookups dentro de Options, inputs copiados literalmente o nulos si nadie los dio, copias `$Context.X` iguales a X, y ninguna propiedad no declarada. Se ignora sin secrets. `SampleContext.AdaLovelace` es el contexto compartido.
- Carpeta espejo del código: `Schema/`, `Plans/`, y las que vengan.
