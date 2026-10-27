namespace DotNet.Practices.SqlErrors.NPlusOne;

// Days is the report's display text ("2026-10-05, 2026-10-06"), so the record compares by value in tests.
public sealed record EmployeeAbsences(string Name, string Days);
