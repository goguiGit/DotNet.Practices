# Facade

Facade en el dominio común de los estructurales (documentos). Explicación para humanos en `README.md`; aquí solo decisiones.

- Fachada `PayslipExporter` en la raíz; subsistema en `Services/`, clases concretas sin interfaces a propósito (el patrón esconde, no abstrae).
- Los servicios son públicos y los tests leen de ellos (`DocumentArchive.Read`, `EmployeeNotifier.Sent`) en vez de usar mocks. No hacerlos `internal`.
- `Export` devuelve la referencia del archivo; no convertirlo en `void`.
- Si la fachada empieza a tener reglas propias, es un servicio de dominio y debe salir de este ejemplo.
- Importe con `FormattableString.Invariant` para que los tests no dependan de la cultura.

Tests en `tests/DotNet.Practices.Patterns.Tests/Structural/Facade/`.
