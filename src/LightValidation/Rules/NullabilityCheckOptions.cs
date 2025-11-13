namespace LightValidation.Rules;

public sealed class NullabilityCheckOptions
{
    public static readonly NullabilityCheckOptions Default = new();

    public bool AllowEmptyStrings { get; init; } = true;

    public bool AllowDefaultValueTypes { get; init; } = true;

    public bool CheckNullableReferenceTypes { get; init; }

    public bool CheckInvalidProperties { get; init; }
}
