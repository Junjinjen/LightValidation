using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("Length", IsPublic = true)]
public sealed class LengthRule<TEntity> : RuleBase<TEntity, string>
{
    private readonly int _minLength;
    private readonly int _maxLength;

    public LengthRule(int minLength, int maxLength)
    {
        if (minLength < 0)
        {
            throw new ArgumentException("The minimum length cannot be less than zero.", nameof(minLength));
        }

        if (maxLength < minLength)
        {
            throw new ArgumentException(
                "The maximum length cannot be less than the minimum length.", nameof(maxLength));
        }

        _minLength = minLength;
        _maxLength = maxLength;
    }

    public override void Configure(IRuleBuildContext<TEntity, string> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.LengthOutOfRange)
            .WithDefaultErrorDescription(
                "The length of \"{PropertyName}\" must be between {MinLength} and {MaxLength} characters " +
                    "[Actual length: {ActualLength}]")
            .WithErrorMetadata("MinLength", _minLength)
            .WithErrorMetadata("MaxLength", _maxLength)
            .WithErrorMetadata("ActualLength", x => x.Length);
    }

    public override bool Validate(ValidationContext<TEntity> context, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= _minLength && value.Length <= _maxLength;
    }
}
