using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Numerics;

namespace LightValidation.Rules.Number;

[ExtensionMethod("LessThanOrEqualTo", IsPublic = true)]
public sealed class LessThanOrEqualToRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : INumber<TProperty>
{
    private readonly TProperty _other;

    public LessThanOrEqualToRule(TProperty other)
    {
        _other = other;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.Number.MustBeLessThanOrEqualToSpecifiedValue)
            .WithDefaultErrorDescription(
                "\"{PropertyName}\" must be less than or equal to {ComparisonValue} [Actual value: {PropertyValue}]")
            .WithErrorMetadata("ComparisonValue", _other);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return value <= _other;
    }
}
