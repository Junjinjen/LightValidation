using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("MaxLength", IsPublic = true)]
public sealed class MaxLengthRule<TEntity> : RuleBase<TEntity, string>
{
    private readonly int _maxLength;

    public MaxLengthRule(int maxLength)
    {
        if (maxLength < 0)
        {
            throw new ArgumentException("The maximum length cannot be less than zero.", nameof(maxLength));
        }

        _maxLength = maxLength;
    }

    public override void Configure(IRuleBuildContext<TEntity, string> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.LengthTooLong)
            .WithDefaultErrorDescription(
                "The length of \"{PropertyName}\" must be equal to or less than {MaxLength} characters " +
                    "[Actual length: {ActualLength}]")
            .WithErrorMetadata("MaxLength", _maxLength)
            .WithErrorMetadata("ActualLength", x => x.Length);
    }

    public override bool Validate(ValidationContext<TEntity> context, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length <= _maxLength;
    }
}
