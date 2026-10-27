# Deadlock

Víctima de deadlock (`SqlException.Number` 1205) reintentada un número acotado de veces. Explicación en `README.md`.

- `LeaveApprovalService.ApproveAsync` repite `ApproveOnceAsync` entera (nueva conexión, nueva transacción) mientras el fallo sea 1205 y queden intentos. Ningún otro número se reintenta.
- `Approval.Attempts` existe para que los tests vean el reintento sin mocks; en producción es la métrica a vigilar.
- El deadlock del test es determinista: el otro proceso hace `SET DEADLOCK_PRIORITY HIGH`, bloquea `LeaveBalances` con `XLOCK, HOLDLOCK`, espera a que el servicio esté bloqueado (polling de `sys.dm_exec_requests`, necesita `VIEW SERVER STATE`) y entonces bloquea `LeaveRequests`. El servicio, con prioridad normal, es siempre la víctima.
- La detección de deadlocks de SQL Server corre cada 5 s como mucho: los tests con deadlock tardan varios segundos por diseño, no por lentitud del código.
- Orden de bloqueo del servicio: `LeaveRequests` y luego `LeaveBalances`. El del otro proceso es el inverso a propósito; no "arreglarlo" en el test, o el deadlock desaparece.

Tests en `tests/DotNet.Practices.SqlErrors.Tests/Deadlock/`.
