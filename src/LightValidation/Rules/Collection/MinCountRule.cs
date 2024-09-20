using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;
using System.Collections;

namespace LightValidation.Rules.Collection;

[ExtensionMethod("MinCount", IsPublic = true)]
public sealed class MinCountRule<TEntity, TProperty> : CountRuleBase<TEntity, TProperty>
    where TProperty : IEnumerable
{
    private readonly int _minCount;

    public MinCountRule(int minCount)
    {
        if (minCount < 0)
        {
            throw new ArgumentException("The minimum count cannot be less than zero.", nameof(minCount));
        }

        _minCount = minCount;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        base.Configure(context);

        context
            .WithDefaultErrorCode(ErrorCode.Collection.CountTooFew)
            .WithDefaultErrorDescription(
                "The count of items in \"{PropertyName}\" cannot be less than {MinCount} " +
                    "[Actual count: {ActualCount}]")
            .WithErrorMetadata("MinCount", _minCount);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        var count = GetCount(value);

        return count >= _minCount;
    }
}
