# Proxy

Proxy en el dominio común de los estructurales (documentos). Explicación para humanos en `README.md`; aquí solo decisiones.

- Sujeto en `Interfaces/IDocumentStore`, sujeto real en `Stores/`, proxies en `Proxies/`. Misma estructura que `Decorator/`; la diferencia es la intención y está en la tabla del README.
- `ArchivedDocumentStore.Loads` existe para que el test de caché cuente viajes sin mocks. No quitarlo por "no es de producción".
- `ProtectedDocumentStoreProxy` no llama al interior cuando deniega: es lo que lo distingue de un decorador. El rol entra por constructor, no por `Read`; no cambiar la interfaz.
- Los dos proxies se encadenan sobre `IDocumentStore`. Si se añade un tercero (lazy loading, logging), misma forma: constructor con `IDocumentStore inner`.
- Un rol distinto de `"HR"` lanza `UnauthorizedAccessException`; es una excepción de la BCL, no envolverla.

Tests en `tests/DotNet.Practices.Patterns.Tests/Structural/Proxy/`.
