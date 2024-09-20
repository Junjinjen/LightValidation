using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Numerics;

namespace LightValidation.Rules.Number;

[ExtensionMethod("GreaterThanOrEqualTo", IsPublic = true)]
public sealed class GreaterThanOrEqualToRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : INumber<TProperty>
{
    private readonly TProperty _other;

    public GreaterThanOrEqualToRule(TProperty other)
    {
        _other = other;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.Number.MustBeGreaterThanOrEqualToSpecifiedValue)
            .WithDefaultErrorDescription(
                "\"{PropertyName}\" must be greater than or equal to {ComparisonValue} " +
                    "[Actual value: {PropertyValue}]")
            .WithErrorMetadata("ComparisonValue", _other);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return value >= _other;
    }
}
