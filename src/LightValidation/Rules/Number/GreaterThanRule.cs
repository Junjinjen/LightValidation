using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Numerics;

namespace LightValidation.Rules.Number;

[ExtensionMethod("GreaterThan", IsPublic = true)]
public sealed class GreaterThanRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : INumber<TProperty>
{
    private readonly TProperty _other;

    public GreaterThanRule(TProperty other)
    {
        _other = other;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.Number.MustBeGreaterThanSpecifiedValue)
            .WithDefaultErrorDescription(
                "\"{PropertyName}\" must be greater than {ComparisonValue} [Actual value: {PropertyValue}]")
            .WithErrorMetadata("ComparisonValue", _other);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return value > _other;
    }
}
