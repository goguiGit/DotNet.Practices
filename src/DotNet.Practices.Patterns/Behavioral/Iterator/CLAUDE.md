# Iterator

Iterator en el dominio común de los de comportamiento (fichaje y ausencias). Explicación para humanos en `README.md`; aquí solo decisiones.

- Agregado `WorkCalendar : IEnumerable<CalendarDay>` en la raíz; iterador a mano en `Iterators/`. Sin interfaz propia `IDayIterator`: se usan `IEnumerable<T>` / `IEnumerator<T>` de la BCL.
- `WorkCalendar` guarda en `SortedDictionary` y eso no se expone. El iterador a mano usa `calendar.GetEnumerator()`, nunca el diccionario; si cambia el almacenamiento, el iterador no se toca.
- `WorkingDays()` (yield) y `WorkingDayIterator` (a mano) son el mismo recorrido a propósito; hay un test que los compara. El de mano existe solo para enseñar la máquina de estados; en producción, yield.
- `CalendarDay` y `DayKind` se repiten en otros ejemplos de la categoría: cada ejemplo es autocontenido. No compartir tipos entre ejemplos.

Tests en `tests/DotNet.Practices.Patterns.Tests/Behavioral/Iterator/`.
