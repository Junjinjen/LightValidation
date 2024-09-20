using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules;

[ExtensionMethod("Defined", IsPublic = true)]
public sealed class DefinedRule<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : struct, Enum
{
    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.ValueMustBeDefinedInEnum)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be a defined enumeration value");
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return Enum.IsDefined(value);
    }
}
