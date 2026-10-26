# Comentarios

Siempre en inglés. El código debe explicarse solo; un comentario solo se justifica cuando el código no puede.

## Sí
- El **por qué** de algo no evidente: una restricción externa, un trade-off deliberado, un workaround.
- Una **trampa**: algo que parece incorrecto o eliminable pero no lo es.
- Enlace a la autoridad de una regla (issue, spec, RFC).

## No
- Repetir lo que dice el código (`// increment i`, `// returns the dog's greeting`).
- Narrar el flujo (`// first we get the user, then...`).
- Banners o separadores (`// --- Helpers ---`, `#region`).
- Código comentado. Se borra; está en git.
- `TODO` sin dueño o sin referencia.
- XML doc (`///`) en miembros cuyo nombre ya lo dice todo. Solo en APIs públicas de librería cuando aportan algo (precondiciones, excepciones).

## Estilo
- Una o dos líneas. Si necesita un párrafo, el problema es el código o el nombre.
- Directo, sin preámbulo ("Note that...", "This method is responsible for...").
- Si cambias el código, actualizas o borras el comentario en el mismo edit.
