using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("Equal", IsPublic = true)]
public sealed class EqualRule<TEntity> : RuleBase<TEntity, string?>
{
    private readonly StringComparison _comparison;
    private readonly string? _expected;

    public EqualRule(string? expected, StringComparison comparison = StringComparison.Ordinal)
    {
        _comparison = comparison;
        _expected = expected;
    }

    public override void Configure(IRuleBuildContext<TEntity, string?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.ValueMustBeEqualToSpecifiedValue)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be equal to {ExpectedValue}")
            .WithErrorMetadata("ExpectedValue", _expected);
    }

    public override bool Validate(ValidationContext<TEntity> context, string? value)
    {
        return string.Equals(value, _expected, _comparison);
    }
}
