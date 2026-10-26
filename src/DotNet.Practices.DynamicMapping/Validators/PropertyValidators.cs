namespace DotNet.Practices.DynamicMapping.Validators;

public record PropertyValidators
{
    public static readonly PropertyValidators None = new();

    public bool Required { get; init; }

    public string? Pattern { get; init; }

    public int? MaxLength { get; init; }

    public bool FutureDate { get; init; }
}
