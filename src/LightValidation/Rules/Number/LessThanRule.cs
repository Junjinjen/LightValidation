using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Numerics;

namespace LightValidation.Rules.Number;

[ExtensionMethod("LessThan", IsPublic = true)]
public sealed class LessThanRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : INumber<TProperty>
{
    private readonly TProperty _other;

    public LessThanRule(TProperty other)
    {
        _other = other;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.Number.MustBeLessThanSpecifiedValue)
            .WithDefaultErrorDescription(
                "\"{PropertyName}\" must be less than {ComparisonValue} [Actual value: {PropertyValue}]")
            .WithErrorMetadata("ComparisonValue", _other);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return value < _other;
    }
}
