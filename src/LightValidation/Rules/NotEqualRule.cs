using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Collections.Generic;

namespace LightValidation.Rules;

[ExtensionMethod("NotEqual", IsPublic = true)]
public sealed class NotEqualRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
{
    private readonly IEqualityComparer<TProperty> _comparer;

    private readonly TProperty _expected;

    public NotEqualRule(TProperty expected, IEqualityComparer<TProperty>? comparer = null)
    {
        _expected = expected;

        _comparer = comparer ?? EqualityComparer<TProperty>.Default;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.ValueCannotBeEqualToSpecifiedValue)
            .WithDefaultErrorDescription("\"{PropertyName}\" cannot be equal to {ExpectedValue}")
            .WithErrorMetadata("ExpectedValue", _expected);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return !_comparer.Equals(value, _expected);
    }
}
