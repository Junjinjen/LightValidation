using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("NotEqual", IsPublic = true)]
public sealed class NotEqualRule<TEntity> : RuleBase<TEntity, string?>
{
    private readonly StringComparison _comparison;
    private readonly string? _expected;

    public NotEqualRule(string? expected, StringComparison comparison = StringComparison.Ordinal)
    {
        _comparison = comparison;
        _expected = expected;
    }

    public override void Configure(IRuleBuildContext<TEntity, string?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.ValueCannotBeEqualToSpecifiedValue)
            .WithDefaultErrorDescription("\"{PropertyName}\" cannot be equal to {ExpectedValue}")
            .WithErrorMetadata("ExpectedValue", _expected);
    }

    public override bool Validate(ValidationContext<TEntity> context, string? value)
    {
        return !string.Equals(value, _expected, _comparison);
    }
}
