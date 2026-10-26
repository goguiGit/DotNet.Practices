# Abstract Factory

**Creacional.** Crear **familias** de objetos relacionados sin acoplarse a sus clases concretas, garantizando que los objetos de una misma familia son compatibles entre sí.

Mismo dominio que el resto de creacionales: logística. Cada vía de transporte es una familia: por carretera se envía en `Truck` con `Pallet`; por mar, en `Ship` con `Container`. Un contenedor marítimo en un camión es la combinación inválida que la fábrica impide. En [refactoring.guru](https://refactoring.guru/es/design-patterns/abstract-factory) el mismo patrón se cuenta con sillas y sofás de distintos estilos.

## Qué hace, en términos de negocio

La misma empresa de logística, un paso más allá. Enviar algo no es solo elegir vehículo: por carretera se carga en **camión** sobre **palés**; por mar, en **barco** dentro de **contenedores**. Camión y contenedor no combinan. Si cada pedido eligiera vehículo y embalaje por separado, antes o después alguien pediría un contenedor para un camión.

Abstract Factory agrupa lo que debe ir junto: se elige **una vía** (carretera o mar) y de ahí salen, a la vez, el vehículo y el embalaje que casan entre sí. El planificador pide "transporte" y "embalaje" a la vía elegida y no puede equivocarse, porque la combinación inválida no se puede ni pedir.

```mermaid
flowchart LR
    V{Vía elegida} -- carretera --> R[Camión + Palés]
    V -- mar --> M[Barco + Contenedores]
    R --> E[Envío planificado<br/>con piezas compatibles]
    M --> E
    X[Camión + Contenedores]:::bad -.-> |imposible de pedir| E
    classDef bad stroke-dasharray: 5 5,color:#999,stroke:#999
```

Quién gana: quien opera, porque desaparecen las combinaciones imposibles; y quien añade una vía nueva (ferrocarril: vagón + plataforma), porque la añade entera sin tocar lo demás.

## Estructura

```mermaid
classDiagram
    class ILogisticsFactory {
        <<interface>>
        +CreateTransport() ITransport
        +CreatePackaging() IPackaging
    }
    class ITransport {
        <<interface>>
        +Mode TransportMode
        +Name string
    }
    class IPackaging {
        <<interface>>
        +Mode TransportMode
        +Name string
        +CanBeLoadedOn(ITransport) bool
    }
    class ShipmentPlanner {
        +Plan() string
    }
    ILogisticsFactory <|.. RoadLogisticsFactory
    ILogisticsFactory <|.. SeaLogisticsFactory
    ITransport <|.. Truck
    ITransport <|.. Ship
    IPackaging <|.. Pallet
    IPackaging <|.. Container
    RoadLogisticsFactory ..> Truck : crea
    RoadLogisticsFactory ..> Pallet : crea
    SeaLogisticsFactory ..> Ship : crea
    SeaLogisticsFactory ..> Container : crea
    ShipmentPlanner --> ILogisticsFactory
```

| Rol | Tipo |
|---|---|
| Abstract factory | `Interfaces/ILogisticsFactory` |
| Concrete factories | `Factories/RoadLogisticsFactory`, `Factories/SeaLogisticsFactory` |
| Abstract products | `Interfaces/ITransport`, `Interfaces/IPackaging` |
| Concrete products | `Transports/*`, `Packagings/*` |
| Client | `ShipmentPlanner` |
| Selector | `LogisticsFactories.For(mode)` |

`ShipmentPlanner` recibe una `ILogisticsFactory` y nunca nombra una clase concreta. Cambiar de vía es cambiar la fábrica que se le pasa. `LogisticsFactories.For` es el único sitio que conoce las fábricas concretas; en una app con DI, ese sitio es el registro del contenedor.

## De Factory Method a Abstract Factory

En [Factory Method](../FactoryMethod/README.md) `Logistics` tenía un solo método fábrica, `CreateTransport()`, y la variante se elegía por **herencia** (`RoadLogistics`, `SeaLogistics`). Aquí aparece un segundo producto que debe combinar con el primero, así que:

| | Factory Method | Abstract Factory |
|---|---|---|
| Qué crea | Un producto (`ITransport`) | Una familia (`ITransport` + `IPackaging`) |
| Mecanismo | Herencia: la subclase sobreescribe el método | Composición: el cliente recibe un objeto fábrica |
| Garantía | Que el cliente no conozca el subtipo | Además, que los productos combinen entre sí |

Cuando un creator empieza a tener dos o tres métodos fábrica relacionados, ya es una Abstract Factory.

## Cuándo usarlo

- El código tiene que trabajar con varias familias y hay combinaciones inválidas (proveedores cloud con sus clientes de cola, blob y secretos; drivers de base de datos con su conexión, comando y transacción; temas de UI).
- Quieres cambiar de familia entera con un solo punto de configuración.

## Cuándo no usarlo

- Hay un solo producto: es Factory Method o una fábrica simple.
- Las familias no tienen restricciones de compatibilidad: inyecta cada producto por separado y deja que el contenedor DI los resuelva.
- Añadir un **producto nuevo** a la familia obliga a tocar la interfaz y todas las fábricas. Si eso va a pasar a menudo, el patrón duele.

## Tests

`LogisticsFactoryTests`: cada fábrica crea sus productos, los productos de una misma fábrica son compatibles, los de fábricas distintas no, y `ShipmentPlanner` funciona con cualquier fábrica sin saber cuál es.
