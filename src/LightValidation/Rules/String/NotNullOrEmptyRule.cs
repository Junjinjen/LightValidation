using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;

namespace LightValidation.Rules.String;

[ExtensionMethod("NotNullOrEmpty", IsPublic = true)]
public sealed class NotNullOrEmptyRule<TEntity> : RuleBase<TEntity, string?>
{
    public override void Configure(IRuleBuildContext<TEntity, string?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.CannotBeNullOrEmpty)
            .WithDefaultErrorDescription("\"{PropertyName}\" cannot be null or empty");
    }

    public override bool Validate(ValidationContext<TEntity> context, string? value)
    {
        return !string.IsNullOrEmpty(value);
    }
}
