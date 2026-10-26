# DotNet.Practices.DynamicMapping

Mapper dinámico de **property bags**: esquemas declarativos que describen un tipo de recurso (una cuenta de dominio, un grupo de distribución, una carpeta compartida) como una lista de propiedades con tipo, validadores, lookups y expresiones que dependen unas de otras. El objetivo es rellenar todas las propiedades a partir de una fuente externa (una BD de RRHH, un formulario) sin escribir el mapeo a mano, y hacerlo de forma **entendible, trazable y reproducible**.

El dominio de los ejemplos es Active Directory, tomado de ficheros reales de un sistema de provisioning. El problema es genérico: cualquier esquema con propiedades calculadas unas a partir de otras.

## El problema

Un property bag real, resumido:

```json
{
  "id": "AdminAdAccountData",
  "properties": [
    { "id": "Domain", "type": "lookup", "lookup": "Lookup.AdAccountDomain", "validators": { "required": true } },
    { "id": "SamAccountName", "type": "text", "unique": "ADAccountData",
      "defaultValue": "$Services.GetFiveTwoOneIdValue(Context.Owner.Lastname, Context.Owner.Firstname, false, '-adm-26')",
      "validators": { "required": true, "pattern": "^[^\\[\\],:;\\|=@\\+\\*\\?<>/\\\\]+$", "maxlength": 20 } },
    { "id": "DisplayName", "type": "text", "valueGenerator": "$Context.SamAccountName", "readonly": true },
    { "id": "DistinguishedName", "type": "text", "readOnly": true,
      "valueGenerator": "$ $'CN={Context.SamAccountName},{Services.GetProvisioningContainer(Context.Options.ProvisioningMapping.Values, 'AdminAdAccountContainer')}'" },
    { "id": "IdForTargetSystem", "type": "text", "readOnly": true,
      "valueGenerator": "$Services.GetIdToCheck(Context.Domain.Key,Context.SamAccountName)" }
  ]
}
```

Tres cosas hacen difícil el mapeo:

1. **Dependencias implícitas.** `DistinguishedName` necesita `SamAccountName`, que necesita datos de la persona (`Context.Owner`). El orden importa y no está escrito en ningún sitio.
2. **Expresiones en un DSL propio.** `$...` es C# script con `Context`, `Services` y `Options` como globals. Interpolaciones `$ $'...'`, llamadas a funciones de negocio, operadores ternarios.
3. **Ficheros reales.** Comas finales, `readonly` y `readOnly` mezclados, `name` como string o como objeto por idioma, defaults que no son strings, bags que heredan de otros con `extends`.

## Grafo de dependencias de `AdminAdAccountData`

Lo que `DependencyGraph` extrae del fichero anterior. Las flechas van de la propiedad a lo que necesita. En gris, lo que viene de fuera del bag.

```mermaid
flowchart LR
    Owner[(Owner<br/>RRHH)]:::ext --> Sam[SamAccountName]
    Options[(Options<br/>contenedores)]:::ext --> DN
    Domain[Domain<br/>lookup]
    Sam --> Display[DisplayName]
    Sam --> DN[DistinguishedName]
    Sam --> IdTS[IdForTargetSystem]
    Domain --> IdTS
    classDef ext fill:#eee,stroke:#999,color:#333
```

Orden de evaluación resultante: `Domain`, `SamAccountName`, `DisplayName`, `AdGuid`, `DistinguishedName`, `IdForTargetSystem`, y después los inputs libres (`UPNOverride`, `StartDate`, `EndDate`, `Comments`, `Description`). El test `PropertyBagOverviewTests` escribe esta tabla en el log para cada bag de cada fixture.

## Arquitectura

La decisión central: **compilar, no interpretar**. El modelo de lenguaje es el compilador en tiempo de diseño; en tiempo de ejecución todo lo que pueda ser determinista lo es.

```mermaid
flowchart TB
    subgraph Diseño["Tiempo de diseño (una vez por tipo)"]
        Bag[propertybags.json] --> Reader[PropertyBagReader]
        Reader --> Model[PropertyBag<br/>Models/]
        Model --> Graph[DependencyGraph<br/>orden + externos]
        Model --> Key[PlanKey<br/>SHA-256 canónico]
        Source[Esquema origen] --> Key
        Graph --> Compiler[RuleCompiler<br/>LLM]
        Compiler --> Plan[MappingPlan<br/>una entrada por propiedad]
    end
    subgraph Ejecución["Tiempo de ejecución (cada registro)"]
        Record[Registro externo] --> Mapper[PropertyBagMapper]
        Plan --> Mapper
        Mapper -->|Expression / Lookup| Det[Evaluación determinista]
        Mapper -->|Dynamic| LLM[LLM + ValidatorGate]
        Det --> Values[Valores + traza]
        LLM --> Values
    end
```

### El plan se cachea, los valores no

`MappingPlan` describe **cómo** se obtiene cada propiedad; depende del esquema, no del registro. La clave es un SHA-256 del canónico JSON de (bag destino, esquema origen, versión del compilador): mismo esquema y mismo compilador, misma clave, da igual el orden de propiedades o columnas; cambiar el prompt del compilador invalida los planes. Nombres multiidioma y mensajes de error quedan fuera porque no afectan al mapeo. No hay TTL: el plan caduca cuando cambia la clave.

```mermaid
flowchart LR
    R[bag + esquema origen] --> K[PlanKey]
    K --> L1{L1<br/>ConcurrentDictionary}
    L1 -- hit --> X[Ejecutar plan]
    L1 -- miss --> L2{L2<br/>fichero JSON revisable}
    L2 -- hit --> L1
    L2 -- miss --> C[Compilar con LLM]
    C --> V[Verificar con ejemplos]
    V --> L2
```

### Tipos de mapeo

| `MappingKind` | Quién lo ejecuta | Ejemplo |
|---|---|---|
| `Expression` | Código, sin modelo | `$Context.SamAccountName` |
| `Lookup` | Código, contra un CSV | `Lookup.AdAccountDomain` |
| `Dynamic` | LLM en cada registro, validado después | Una descripción libre |
| `Unmapped` | Nadie: lo revisa una persona | El compilador no supo |

## Estado actual

| Pieza | Carpeta | Estado |
|---|---|---|
| Modelos del esquema | `Models/` | hecho |
| Lector de `propertybags.json` | `Schema/` | hecho, con 4 ficheros reales como fixtures |
| Validadores declarados y ejecutables | `Validators/` | hecho: tipo, `required`, `pattern`, `maxlength`, `futureDate` |
| Grafo de dependencias y orden | `Dependencies/` | hecho |
| `MappingPlan` y `PlanKey` | `Plans/` | hecho |
| Línea base con LLM | `Resolution/LlmPropertyBagResolver` | hecho |
| Resolución de `extends` | | pendiente, el bag base está fuera del repo |
| Mini-DSL determinista (`ExpressionResolver`) | | pendiente |
| Caché L1/L2 del plan (`MappingPlanner`, `Stores/`) | | pendiente |
| Compilador de planes (`RuleCompiler`) | | pendiente |

### La línea base: el modelo evalúa todo

`LlmPropertyBagResolver` es el experimento que responde a la pregunta original: ¿puede el modelo resolver las dependencias entre propiedades él solo? Recibe el registro externo, los inputs del solicitante, los contenedores y lookups, la semántica de `Services.*` **en lenguaje natural** y las propiedades en orden de evaluación, y devuelve un JSON con todos los valores.

```mermaid
sequenceDiagram
    participant T as Test
    participant R as LlmPropertyBagResolver
    participant G as DependencyGraph
    participant M as Azure OpenAI
    participant V as PropertyValidator
    T->>R: ResolveAsync(bag, context)
    R->>G: EvaluationOrder()
    R->>M: system: reglas + user: Owner, Inputs, Options, Services, propiedades en orden
    M-->>R: { "Domain": "qaacc.int", "SamAccountName": "lovel-adm-26", ... }
    R-->>T: ResolvedBag
    T->>V: Validate(property, value) por cada propiedad
    V-->>T: sin violaciones
```

Resultado con gpt-4o sobre los cuatro tipos: **todas las propiedades válidas**. Pero la misma regla `GetFiveTwoOneIdValue` produjo `lovel-adm-26` en un bag y `lovelada26` en otro; ninguno es el 5+2 exacto. Y sin `Inputs`, el modelo dejó nulos `Name` y `PrimarySmtpAddressLocalPart` en vez de inventarlos, que es lo correcto. Las dos cosas confirman el diseño: el modelo entiende el esquema y las dependencias, pero un identificador no puede depender de que hoy lo calcule igual que ayer. Por eso el camino de ejecución será el plan compilado, y esta clase queda como referencia.

## Tests

```
dotnet test tests/DotNet.Practices.DynamicMapping.Tests --filter "Category!=Integration"
dotnet test tests/DotNet.Practices.DynamicMapping.Tests --filter "Category=Integration" --logger "console;verbosity=detailed"
```

- `Fixtures/PropertyBags/`: cuatro `propertybags.json` reales (`AdminAdAccount`, `StandardAdAccount`, `SharedFolder`, `DistributionGroup`). Una clase de tests por fichero compara cada propiedad con el record esperado completo.
- `PropertyBagOverviewTests`: un caso por bag, escribe en el log propiedades, dependencias y orden; asserta que no hay ciclos.
- `LlmPropertyBagResolverIntegrationTests`: un caso por bag, resuelve todas las propiedades con el modelo real (JSON forzado, temperatura 0) y asserta en una sola lista de violaciones: validadores, lookups dentro del catálogo, inputs copiados literalmente, copias `$Context.X` consistentes y ninguna propiedad inventada. Lee `OpenAI:Endpoint`, `OpenAI:ApiKey`, `OpenAI:DeploymentName` de user-secrets (`DotNet.Practices.Secrets`) y se ignora si no están.
- Un `Assert.That` por test. Sin mocks: `FakeChatClient` devuelve una respuesta fija y guarda el prompt.

## Decisiones

Están en [`CLAUDE.md`](CLAUDE.md), junto con el orden de los pasos pendientes. Las que más cuesta ver desde el código: por qué los nombres multiidioma no entran en la clave, por qué `Type` puede ser `null`, por qué el canónico de la clave ordena propiedades y columnas, y por qué la semántica de `Services.*` del prompt es supuesta y no la real.
