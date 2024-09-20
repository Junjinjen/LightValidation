using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;
using System.Collections;

namespace LightValidation.Rules.Collection;

[ExtensionMethod("ExactCount", IsPublic = true)]
public sealed class ExactCountRule<TEntity, TProperty> : CountRuleBase<TEntity, TProperty>
    where TProperty : IEnumerable
{
    private readonly int _count;

    public ExactCountRule(int count)
    {
        if (count < 0)
        {
            throw new ArgumentException("The count cannot be less than zero.", nameof(count));
        }

        _count = count;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        base.Configure(context);

        context
            .WithDefaultErrorCode(ErrorCode.Collection.CountMustBeExact)
            .WithDefaultErrorDescription(
                "The count of items in \"{PropertyName}\" must be exactly {ExactCount} [Actual count: {ActualCount}]")
            .WithErrorMetadata("ExactCount", _count);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        var count = GetCount(value);

        return count == _count;
    }
}
