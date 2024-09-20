using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Numerics;

namespace LightValidation.Rules.Number;

[ExtensionMethod("ExclusiveBetween", IsPublic = true)]
public sealed class ExclusiveBetweenRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : INumber<TProperty>
{
    private readonly TProperty _minValue;
    private readonly TProperty _maxValue;

    public ExclusiveBetweenRule(TProperty minValue, TProperty maxValue)
    {
        _minValue = minValue;
        _maxValue = maxValue;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.Number.MustBeBetweenExclusive)
            .WithDefaultErrorDescription(
                "\"{PropertyName}\" must be between {MinValue} and {MaxValue}, exclusive " +
                    "[Actual value: {PropertyValue}]")
            .WithErrorMetadata("MinValue", _minValue)
            .WithErrorMetadata("MaxValue", _maxValue);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return value > _minValue && value < _maxValue;
    }
}
