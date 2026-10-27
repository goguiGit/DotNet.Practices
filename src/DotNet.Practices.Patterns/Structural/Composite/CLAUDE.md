# Composite

Ejemplo del patrón Composite: cadena de mando como árbol.

- `Rank`: enum ordenado de menor a mayor (`Private` → `Captain`). El orden importa: las comparaciones de rango usan `>` y `>=`.
- `Order`: enum de órdenes (`Salute`, `Shoot`). Tipada a propósito, no `string`.
- `Soldier`: hoja y compuesto a la vez. `AddSubordinate` exige que quien manda no sea `Private` y que el súbdito tenga rango estrictamente inferior. `GiveOrder` baja recursivamente por todo el árbol; quien la da no la recibe.

Decisiones ya tomadas:
- Composite, no Chain of Responsibility (la orden baja, no sube) ni Command (sin `Execute`/`Undo`, el enum basta).
- Violaciones de rango lanzan excepción: son errores del llamador.
- `ReceivedOrders` existe para que los tests verifiquen el alcance sin mocks.

Tests en `tests/DotNet.Practices.Patterns.Tests/Structural/Composite/`.
