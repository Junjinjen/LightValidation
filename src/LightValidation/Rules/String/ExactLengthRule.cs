using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("ExactLength", IsPublic = true)]
public sealed class ExactLengthRule<TEntity> : RuleBase<TEntity, string>
{
    private readonly int _length;

    public ExactLengthRule(int length)
    {
        if (length < 0)
        {
            throw new ArgumentException("The length cannot be less than zero.", nameof(length));
        }

        _length = length;
    }

    public override void Configure(IRuleBuildContext<TEntity, string> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.LengthMustBeExact)
            .WithDefaultErrorDescription(
                "The length of \"{PropertyName}\" must be exactly {ExactLength} characters " +
                    "[Actual length: {ActualLength}]")
            .WithErrorMetadata("ExactLength", _length)
            .WithErrorMetadata("ActualLength", x => x.Length);
    }

    public override bool Validate(ValidationContext<TEntity> context, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length == _length;
    }
}
