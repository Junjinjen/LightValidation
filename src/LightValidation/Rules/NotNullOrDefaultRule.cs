using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using System.Collections.Generic;

namespace LightValidation.Rules;

public sealed class NotNullOrDefaultRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
{
    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.ValueCannotBeNullOrDefault)
            .WithDefaultErrorDescription("\"{PropertyName}\" cannot be null or have a default value");
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return !EqualityComparer<TProperty>.Default.Equals(value, default);
    }
}
