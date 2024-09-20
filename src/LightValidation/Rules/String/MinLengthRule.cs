using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("MinLength", IsPublic = true)]
public sealed class MinLengthRule<TEntity> : RuleBase<TEntity, string>
{
    private readonly int _minLength;

    public MinLengthRule(int minLength)
    {
        if (minLength < 0)
        {
            throw new ArgumentException("The minimum length cannot be less than zero.", nameof(minLength));
        }

        _minLength = minLength;
    }

    public override void Configure(IRuleBuildContext<TEntity, string> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.LengthTooShort)
            .WithDefaultErrorDescription(
                "The length of \"{PropertyName}\" must be at least {MinLength} characters " +
                    "[Actual length: {ActualLength}]")
            .WithErrorMetadata("MinLength", _minLength)
            .WithErrorMetadata("ActualLength", x => x.Length);
    }

    public override bool Validate(ValidationContext<TEntity> context, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= _minLength;
    }
}
