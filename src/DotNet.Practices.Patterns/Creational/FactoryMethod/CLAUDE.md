# FactoryMethod

Factory Method con el dominio de refactoring.guru (logística). La explicación para humanos está en `README.md`; aquí solo decisiones.

- Clase base en `Abstracts/Logistics`, creators concretos en `Creators/`, productos en `Transports/`, contrato en `Interfaces/ITransport`.
- No usar una carpeta `Logistics/` para los creators: carpeta y clase con el mismo nombre producen el choque `Logistics.Logistics`.
- `PlanDelivery` existe a propósito: sin lógica en el creator, el patrón sobra (ver README).

Tests en `tests/DotNet.Practices.Patterns.Tests/Creational/FactoryMethod/`.
