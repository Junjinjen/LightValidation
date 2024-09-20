using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;
using System.Collections;

namespace LightValidation.Rules.Collection;

[ExtensionMethod("MaxCount", IsPublic = true)]
public sealed class MaxCountRule<TEntity, TProperty> : CountRuleBase<TEntity, TProperty>
    where TProperty : IEnumerable
{
    private readonly int _maxCount;

    public MaxCountRule(int maxCount)
    {
        if (maxCount < 0)
        {
            throw new ArgumentException("The maximum count cannot be less than zero.", nameof(maxCount));
        }

        _maxCount = maxCount;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        base.Configure(context);

        context
            .WithDefaultErrorCode(ErrorCode.Collection.CountTooMany)
            .WithDefaultErrorDescription(
                "The count of items in \"{PropertyName}\" cannot exceed {MaxCount} [Actual count: {ActualCount}]")
            .WithErrorMetadata("MaxCount", _maxCount);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        var count = GetCount(value);

        return count <= _maxCount;
    }
}
