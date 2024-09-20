using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;
using System.Collections;

namespace LightValidation.Rules.Collection;

[ExtensionMethod("Count", IsPublic = true)]
public sealed class CountRule<TEntity, TProperty> : CountRuleBase<TEntity, TProperty>
    where TProperty : IEnumerable
{
    private readonly int _minCount;
    private readonly int _maxCount;

    public CountRule(int minCount, int maxCount)
    {
        if (minCount < 0)
        {
            throw new ArgumentException("The minimum count cannot be less than zero.", nameof(minCount));
        }

        if (maxCount < minCount)
        {
            throw new ArgumentException("The maximum count cannot be less than the minimum count.", nameof(maxCount));
        }

        _minCount = minCount;
        _maxCount = maxCount;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        base.Configure(context);

        context
            .WithDefaultErrorCode(ErrorCode.Collection.CountOutOfRange)
            .WithDefaultErrorDescription(
                "The count of items in \"{PropertyName}\" must be between {MinCount} and {MaxCount} " +
                    "[Actual count: {ActualCount}]")
            .WithErrorMetadata("MinCount", _minCount)
            .WithErrorMetadata("MaxCount", _maxCount);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        var count = GetCount(value);

        return count >= _minCount && count <= _maxCount;
    }
}
