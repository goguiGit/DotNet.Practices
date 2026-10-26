# DotNet.Practices.ScriptedMapping

Cómo lo resolvía el sistema original (Rabbit, provisioning de identidades) y por qué `DynamicMapping` cambia lo que cambia. Este proyecto es una reimplementación mínima y fiel del mecanismo de Rabbit, con tests sobre los mismos property bags reales, para que la comparación sea código contra código y no opinión contra opinión.

Los porqués de Rabbit están deducidos de su código (`src/Algacom.Rabbit.R2.Kernel/PropertyBags/DynamicExpressions`, `src/Algacom.Rabbit.Infrastructure/Configuration/PropertyBags`), no de documentación: Rabbit no la tiene.

## Cómo lo hace Rabbit

```mermaid
flowchart TB
    subgraph Arranque["Arranque del servicio (una vez)"]
        J[propertybags.json<br/>+ bags en C#] --> Reg[PropertyBagTypeRegistry<br/>resuelve extends]
        Reg --> Comp["Por cada propiedad con $:<br/>quitar $, ' → \"<br/>CSharpScript.ContinueWith().CreateDelegate()"]
        Comp --> Deps[Dependencias por<br/>modelo semántico de Roslyn]
        Comp --> Cache[(ConcurrentDictionary<br/>por tipo y propiedad)]
    end
    subgraph Peticion["Cada alta o cambio"]
        Cmd[Comando] --> Ctx["ScriptGlobals { dynamic Context, dynamic Services }"]
        Cache --> Loop
        Ctx --> Loop["Bucle: primera propiedad con dependencias listas<br/>→ ejecutar → reiniciar"]
        Loop --> Val[Validadores en servidor]
        Val --> Store[(Guardar)]
    end
```

### Decisiones del diseño original y su porqué

| Decisión | Porqué (deducido del código) | Coste que se paga |
|---|---|---|
| **Expresiones en C# script con Roslyn** | Toda la potencia de C# sin escribir un intérprete: operadores ternarios, `?.`, interpolación, llamadas a servicios. Los consultores configuran sin recompilar el producto. | Sin sandbox: quien escribe en la carpeta de configuración ejecuta código con confianza total. Cada script arrastra el ensamblado completo de infraestructura. |
| **`$` como marca y `'` → `"`** | Un JSON no puede contener `"` sin escapar; con `'` las expresiones se leen. El `$` distingue script de literal. | No hay forma de escribir un `char` ni un apóstrofo en una expresión. `"123"` sin `$` se parsea como `int` y revienta al asignarse a un texto. |
| **`Context` y `Services` como `dynamic`** | Los nombres de propiedades y servicios vienen de configuración; con `dynamic` el script compila sin generar un tipo por bag. | Un error de nombre solo aparece en runtime. Rabbit lo traga como `RuntimeBinderException`, lo loguea y deja la propiedad sin valor: el fallo es silencioso. |
| **Compilar todo en el arranque** | Un error de configuración tira el servicio antes de servir una petición. El coste de Roslyn se paga una vez. | Arranque lento. Un bag compartido por varios tipos se compila varias veces y se descarta. Cambiar la configuración exige reiniciar. |
| **Orden por bucle "scan-and-restart"** | Sin construir un grafo: busca la primera propiedad cuyas dependencias ya estén, la ejecuta y vuelve al principio. Trivial de escribir. | O(n²) en cada petición. Los ciclos solo se detectan en runtime, y el contador del detector compara contra el total, no contra lo pendiente: con una propiedad ya evaluada antes de llegar al ciclo, lanza `ArgumentOutOfRangeException`. |
| **Dependencias por análisis semántico de Roslyn** | Precisas: un nombre ambiguo entre bags del mismo tipo es un error de configuración, no una sorpresa. | Las dependencias se calculan con los bags del primer tipo de configuración que compila; otro tipo con bags distintos las hereda sin saberlo. |
| **`extends` fusionado en el registro** | Reutilizar un bag base (`AdObjectAccessTypePropertyBag`, que es una clase C#) y sobreescribir solo `defaultValue`. | Al sobreescribir una propiedad no se copian `IdentifyingGenerator`, `Sensitive`, `Description` ni `ResetDefaultValueOnDependenciesChanges`: sobreescribirlos no tiene efecto. Validadores se unen y no se pueden quitar en el hijo. |
| **Validadores en servidor tras evaluar, un subconjunto en cliente** | El servidor es la verdad; el cliente solo da feedback inmediato con `required`, `pattern`, `minlength`, `maxlength`. | `FutureDate`, `emailDomain` y `range` solo fallan al enviar. `pattern` construye un `Regex` nuevo en cada validación. `required` pasa con valor nulo si hay `defaultValue`. |
| **Secuencias en base de datos para ids únicos** | `GetFiveTwoOneIdValue` y `GetNextIdValue` reservan globalmente y hacen bucle hasta encontrar un valor libre. Garantía real de unicidad entre instancias. | Consultas a base de datos dentro de la evaluación de una propiedad; en preview, consultas sin reserva. |

### La regla "5-2-1", exacta

`GetFiveTwoOneIdValue(first, second, upper, suffix)`: quita espacios (solo espacios), pasa a minúsculas o mayúsculas, toma 5 caracteres del apellido y 2 del nombre, añade un contador en base 36 **empezando en 1 y siempre en mayúsculas**, y el sufijo. Cuando el contador necesita dos caracteres, el nombre se reduce a uno para que el id siga teniendo ocho caracteres antes del sufijo. A los 1296 lanza.

| Llamada | Resultado |
|---|---|
| `("Lovelace", "Ada", false, "-adm-26")` | `lovelad1-adm-26` |
| décima vez | `loveladA-adm-26` |
| trigésimo sexta vez | `lovela10-adm-26` |

El modelo de lenguaje, con la misma regla descrita en lenguaje natural, produjo `lovel-adm-26` y `lovelada26`. Ninguno es correcto. Esa diferencia es la razón de ser de `DynamicMapping`.

## Qué replica este proyecto y qué deja fuera

| Replica | Deja fuera, y por qué |
|---|---|
| Traducción `$` → script, Roslyn scripting, delegado cacheado por bag y propiedad | El registro de tipos y la fusión de `extends`: el bag base vive en C# en Rabbit y no en el repo |
| `ScriptGlobals` con `Context` y `Services` dinámicos, `Owner` y `Options` anidados | Validadores, backfeed, `Relations`, `Variables`, `Command`: no cambian la comparación |
| Bucle scan-and-restart, `UnresolvedPropertyValue`, `RuntimeBinderException` tragada | Secuencias en base de datos: un `InMemorySequenceTracker` basta para ver la mecánica |
| `ParseValue` de literales (bool, int, double, texto) | El análisis semántico para dependencias: se reutiliza el extractor por regex de `DynamicMapping` |
| Semántica exacta de `GetFiveTwoOneIdValue`, `GetProvisioningContainer`, `GetIdToCheck`, `GetNextIdValue`, `Combine` | |

## Mejoras significativas de `DynamicMapping`, con su porqué

```mermaid
flowchart LR
    subgraph Rabbit
        R1[C# script sin sandbox] --> R2[dynamic: errores en runtime]
        R2 --> R3[orden O n² por petición]
        R3 --> R4[sin clave: recompila<br/>y reinicia para cambiar]
    end
    subgraph DynamicMapping
        D1[Mini-DSL cerrado<br/>o plan compilado] --> D2[Grafo de dependencias<br/>y ciclos en diseño]
        D2 --> D3[PlanKey: cambia el esquema,<br/>cambia el plan]
        D3 --> D4[Validadores ejecutables<br/>con la misma definición]
    end
    Rabbit -. mismo propertybags.json .-> DynamicMapping
```

1. **Dependencias y ciclos en tiempo de diseño, no de petición.** Rabbit descubre el orden en cada alta con un bucle cuadrático y detecta los ciclos cuando ya está sirviendo. `DependencyGraph` lo calcula una vez, con orden estable y una excepción que nombra solo a los miembros del ciclo. El porqué: el orden es una propiedad del esquema, no del registro; calcularlo por petición es trabajo repetido y esconde errores de configuración hasta producción.
2. **El esquema produce una clave.** Rabbit no tiene noción de "este bag ha cambiado": recompila todo al arrancar y hay que reiniciar para que un cambio entre. `PlanKey` convierte bag, origen y versión del compilador en un hash: lo que no cambia se reutiliza, lo que cambia se recompila, sin reinicio. El porqué: la caché necesita una identidad, y la identidad correcta es el contenido canónico, no el nombre del fichero.
3. **Fallar explícito en vez de callar.** Rabbit traga el `RuntimeBinderException` y deja la propiedad vacía; el usuario ve un campo en blanco sin saber por qué. `PropertyValidator` devuelve la lista de violaciones y el test de integración exige que cada propiedad tenga valor o una razón. El porqué: en un sistema de identidades un valor ausente en silencio es peor que un error ruidoso.
4. **Lector tolerante probado contra los ficheros reales.** Rabbit falla al arrancar con un JSON estricto roto y convierte `"123"` en entero. `PropertyBagReader` acepta lo que los ficheros reales traen (coma final, `readonly`/`readOnly`, defaults no string) y lo documenta con una clase de tests por fichero. El porqué: la configuración real es la que es; el lector se adapta a ella, no al revés.
5. **El modelo de lenguaje compila, no interpreta.** Rabbit no tiene LLM; su equivalente es el consultor que escribe el script. `DynamicMapping` propone que el modelo traduzca reglas en lenguaje natural a expresiones deterministas una vez, y que el runtime no dependa de él. El porqué está en la tabla del 5-2-1: el mismo modelo dio dos resultados distintos y ambos incorrectos para la misma regla.
6. **Sin ejecución de código arbitrario.** El mini-DSL previsto admite interpolación, `Context.X` y un catálogo cerrado de funciones. El porqué: en Rabbit, escribir en la carpeta de configuración equivale a desplegar código; un DSL cerrado hace que la configuración siga siendo configuración.

Lo que Rabbit hace mejor y `DynamicMapping` todavía no tiene: unicidad real de ids con secuencias reservadas en base de datos, resolución de `extends`, y un catálogo de validadores mucho más amplio (`e164`, `range`, `timespan`, `multiEmail`, validadores en `.csx`).

## Tests

```
dotnet test tests/DotNet.Practices.ScriptedMapping.Tests
```

- `ExpressionTranslatorTests`: la traducción `$` → script.
- `ScriptServicesTests`: la semántica exacta de cada servicio, incluidos los casos de contador a dos caracteres y de mayúsculas.
- `ScriptedPropertyBagEvaluatorTests`: `AdminAdAccount` real evaluado con Roslyn (`lovelad1-adm-26`, `CN=lovelad1-adm-26,OU=Admins,DC=qaacc,DC=int`, `qaacc.int\lovelad1-adm-26`), determinismo entre ejecuciones, orden con dependencias declaradas después, literales parseados como Rabbit, ciclo, caché de compilación, error de compilación con nombre de propiedad y el null tragado como en Rabbit.
- Los fixtures son los mismos ficheros que `DynamicMapping`, enlazados desde su proyecto de tests.
