# Implementación

## Lo mínimo que cumple el requisito actual
- Elegir la implementación más simple que cumpla lo pedido hoy. No construir para requisitos hipotéticos: ni parámetros "por si acaso", ni abstracciones sin segundo consumidor, ni configuración que nadie pidió.
- Antes de escribir código nuevo: reutilizar lo que ya hay en el repo, luego la BCL, luego una dependencia ya instalada. Añadir un paquete nuevo exige decirlo y justificarlo.
- Un cambio termina cuando llega a todo lo que toca: llamadores, tests, fixtures, documentación (`README.md` y `CLAUDE.md` de la carpeta). Parar antes es dejar el trabajo a medias, no ser conciso.

## Cuándo preguntar
- Solo cuando una elección sin resolver cambia materialmente el comportamiento, la compatibilidad, la seguridad de los datos o la autorización. Todo lo demás se decide, se hace y se explica en el resumen.
- Si una pregunta no bloquea el resto, se hace primero todo lo que no depende de ella.

## Seguridad al verificar
- Que un test tenga configuración de conexión no demuestra que la base de datos, la cuenta de almacenamiento o el endpoint sean desechables. Antes de una operación que muta estado externo (borrar, truncar, enviar, reservar), comprobar la identidad del destino en la configuración o en los user-secrets y decirlo.
- Los tests de integración contra servicios reales llevan `[Category("Integration")]`, leen credenciales de user-secrets y hacen `Assert.Ignore` si faltan. Nunca credenciales en código ni en `appsettings*.json`.
- Un test que consume tokens o cuota se ejecuta a propósito (`--filter "Category=Integration"`), no por accidente.

## Introducido frente a preexistente
- Al revisar o al cerrar un cambio, distinguir los problemas que introduce el cambio de los que ya estaban. Los primeros se arreglan antes de dar por terminado; los segundos se anotan y se deciden aparte.
