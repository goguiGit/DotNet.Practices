namespace DotNet.Practices.SqlErrors.Deadlock;

public sealed record Approval(int RequestId, int Attempts);
