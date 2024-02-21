using System.Collections.Frozen;
using System.Collections.Generic;

namespace LightValidation.Result;

public class RuleFailure
{
    public required string PropertyName { get; init; }

    public required object? PropertyValue { get; init; }

    public required string ErrorCode { get; init; }

    public required string ErrorDescription { get; init; }

    public IReadOnlyDictionary<string, object?> ErrorMetadata { get; init; } = FrozenDictionary<string, object?>.Empty;

    public override string ToString()
    {
        return ErrorDescription;
    }
}
