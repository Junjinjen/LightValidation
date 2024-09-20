using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Numerics;

namespace LightValidation.Rules.Number;

[ExtensionMethod("InclusiveBetween", IsPublic = true)]
public sealed class InclusiveBetweenRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : INumber<TProperty>
{
    private readonly TProperty _minValue;
    private readonly TProperty _maxValue;

    public InclusiveBetweenRule(TProperty minValue, TProperty maxValue)
    {
        _minValue = minValue;
        _maxValue = maxValue;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.Number.MustBeBetweenInclusive)
            .WithDefaultErrorDescription(
                "\"{PropertyName}\" must be between {MinValue} and {MaxValue}, inclusive " +
                    "[Actual value: {PropertyValue}]")
            .WithErrorMetadata("MinValue", _minValue)
            .WithErrorMetadata("MaxValue", _maxValue);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return value >= _minValue && value <= _maxValue;
    }
}
