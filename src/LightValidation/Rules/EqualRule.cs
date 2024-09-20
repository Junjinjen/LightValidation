using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System.Collections.Generic;

namespace LightValidation.Rules;

[ExtensionMethod("Equal", IsPublic = true)]
public sealed class EqualRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
{
    private readonly IEqualityComparer<TProperty> _comparer;

    private readonly TProperty _expected;

    public EqualRule(TProperty expected, IEqualityComparer<TProperty>? comparer = null)
    {
        _expected = expected;

        _comparer = comparer ?? EqualityComparer<TProperty>.Default;
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.ValueMustBeEqualToSpecifiedValue)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be equal to {ExpectedValue}")
            .WithErrorMetadata("ExpectedValue", _expected);
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return _comparer.Equals(value, _expected);
    }
}
